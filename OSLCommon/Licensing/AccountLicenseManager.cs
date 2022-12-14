using kate.shared.Helpers;
using OSLCommon.Authorization;
using OSLCommon.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSLCommon.Licensing
{
    public class AccountLicenseManager
    {
        public AccountManager AccountManager { get; private set; }
        public AccountLicenseManager(AccountManager accountManager)
        {
            AccountManager = accountManager;
        }

        #region Events

        /// <param name="license">Nullable <see cref="LicenseKeyMetadata"/></param>
        public event LicenseFieldDelegate Update;
        public event LicenseGroupDelegate GroupUpdate;
        public event LicenseDelegate LicenseUpdate;
        public void OnUpdate(LicenseField field, LicenseKeyMetadata license)
        {
            if (Update != null)
            {
                if (license != null)
                {
                    HookLicenseEvent(license);
                    if (LicenseUpdate != null)
                        LicenseUpdate?.Invoke(license);
                }
                Update?.Invoke(field, license);
            }
        }
        public void OnGroupUpdate(LicenseGroup group)
        {
            if (GroupUpdate != null)
                GroupUpdate?.Invoke(group);
        }
        public void OnLicenseUpdate(LicenseKeyMetadata license)
        {
            if (LicenseUpdate != null)
                LicenseUpdate?.Invoke(license);
        }

        public virtual void RefreshHook()
        {
            foreach (var pair in LicenseKeys)
            {
                HookLicenseEvent(pair.Value);
            }
            foreach (var pair in LicenseGroups)
            {
                HookLicenseGroupEvent(pair.Value);
            }
        }
        #endregion

        #region License (|Group) Merge/Hook
        protected virtual void HookLicenseEvent(LicenseKeyMetadata target)
        {
            target.manager = this;
            if (target == null || target.eventHook) return;
            target.eventHook = true;
            LicenseUpdate += (source) =>
            {
                LicenseMerge(source, target);
            };
            Update += (field, source) =>
            {
                LicenseMerge(source, target);
            };
        }
        protected virtual void LicenseMerge(LicenseKeyMetadata source, LicenseKeyMetadata target)
        {
            if (source != null && source.UID == target.UID)
            {
                source.Merge(target);
            }
        }
        protected virtual void HookLicenseGroupEvent(LicenseGroup target)
        {
            target.manager = this;
            if (target == null || target.eventHook) return;
            target.eventHook = true;
            GroupUpdate += (source) =>
            {
                LicenseGroupMerge(source, target);
            };
        }
        protected virtual void LicenseGroupMerge(LicenseGroup source, LicenseGroup target)
        {
            if (source != null && source.UID == target.UID)
            {
                target.Merge(source);
            }
        }
        #endregion

        public Dictionary<string, LicenseKeyMetadata> LicenseKeys { get; set; } = new Dictionary<string, LicenseKeyMetadata>();
        public Dictionary<string, LicenseGroup> LicenseGroups { get; set; } = new Dictionary<string, LicenseGroup>();

        #region Get/Set License/Groups
        /// <returns>Nullable <see cref="LicenseKeyMetadata"/></returns>
        public virtual async Task<LicenseKeyMetadata> GetLicenseKey(string key, bool hook = true)
        {
            var match = LicenseHelper.LicenseKeyRegex.Match(key);
            if (!match.Success) return null;
            foreach (var item in LicenseKeys)
                if (item.Value.Key == key)
                {
                    if (hook)
                        HookLicenseEvent(item.Value);
                    return item.Value;
                }
            return null;
        }
        public virtual async Task<LicenseKeyMetadata[]> GetLicenseKeys(bool hook = true)
        {
            var list = new List<LicenseKeyMetadata>();
            foreach (var pair in LicenseKeys)
            {
                if (hook)
                    HookLicenseEvent(pair.Value);
                list.Add(pair.Value);
            }
            return list.ToArray();
        }
        /// <param name="overwrite">Delete license keys not given.</param>
        public virtual async Task SetLicenseKeys(LicenseKeyMetadata[] keys, bool overwrite = true)
        {
            Dictionary<string, LicenseKeyMetadata> newDictionary = new Dictionary<string, LicenseKeyMetadata>();
            if (!overwrite)
                newDictionary = LicenseKeys;
            foreach (var item in keys)
            {
                if (newDictionary.ContainsKey(item.UID))
                    newDictionary[item.UID].Merge(item);
                else
                {
                    HookLicenseEvent(item);
                    newDictionary.Add(item.UID, item);
                }
            }
            LicenseKeys = newDictionary;
            OnUpdate(LicenseField.All, null);
        }
        public virtual async Task DeleteLicenseKey(string keyId)
        {
            if (LicenseKeys.ContainsKey(keyId))
                LicenseKeys.Remove(keyId);
            foreach (var item in LicenseGroups)
            {
                item.Value.LicenseIds = item.Value.LicenseIds.Where(v => v != keyId).ToArray();
            }
            OnUpdate(LicenseField.All, null);
        }

        public virtual async Task<LicenseGroup[]> GetGroups(bool hook = true)
        {
            var list = new List<LicenseGroup>();
            foreach (var pair in LicenseGroups)
            {
                if (hook)
                    HookLicenseGroupEvent(pair.Value);
                list.Add(pair.Value);
            }
            return list.ToArray();
        }
        public virtual async Task SetGroups(LicenseGroup[] groups, bool overwrite = true, bool deleteKeys = true)
        {
            Dictionary<string, LicenseGroup> dict = new Dictionary<string, LicenseGroup>();
            if (!overwrite)
                dict = LicenseGroups;
            var groupIdList = new List<string>();
            foreach (var item in groups)
            {
                groupIdList.Add(item.UID);
                if (dict.ContainsKey(item.UID))
                    dict[item.UID].Merge(item);
                else
                {
                    HookLicenseGroupEvent(item);
                    dict.Add(item.UID, item);
                }
            }
            LicenseGroups = dict;
            if (deleteKeys)
            {
                LicenseKeys = LicenseKeys
                    .Where(v => groupIdList.Contains(v.Value.GroupId))
                    .ToDictionary(x => x.Key, v => v.Value);
            }
            OnUpdate(LicenseField.All, null);
        }
        /// <param name="includeKeys">Delete License Keys associated with group</param>
        public virtual async Task DeleteGroup(string groupId, bool includeKeys = true)
        {
            if (LicenseGroups.ContainsKey(groupId))
                LicenseGroups.Remove(groupId);
            if (includeKeys)
            {
                var keyList = new List<string>();
                foreach (var pair in LicenseKeys)
                {
                    if (pair.Value.GroupId == groupId)
                        keyList.Add(pair.Key);
                }
                foreach (var item in keyList)
                    await DeleteLicenseKey(item);
            }
            OnUpdate(LicenseField.All, null);
        }

        public virtual async Task<AccountLicenseManagerSummary> GetSummary()
        {
            return new AccountLicenseManagerSummary
            {
                Keys = GetLicenseKeys().Result,
                Groups = GetGroups().Result
            };
        }
        #endregion

        public enum LicenseKeyActionResult
        {
            Invalid = -1,
            Failure  = 0,
            Success  = 1,
        }
        public virtual async Task<LicenseKeyActionResult> DisableLicenseKey(string keyId)
        {
            var match = LicenseHelper.LicenseIdRegex.Match(keyId);
            if (!match.Success) return LicenseKeyActionResult.Invalid;
            if (!LicenseKeys.ContainsKey(keyId))
                return LicenseKeyActionResult.Invalid;

            LicenseKeys[keyId].Enable = false;
            OnUpdate(LicenseField.Enable, LicenseKeys[keyId]);
            return LicenseKeyActionResult.Success;
        }
        public virtual async Task<LicenseKeyActionResult> EnableLicenseKey(string keyId)
        {
            var match = LicenseHelper.LicenseIdRegex.Match(keyId);
            if (!match.Success) return LicenseKeyActionResult.Invalid;
            if (!LicenseKeys.ContainsKey(keyId))
                return LicenseKeyActionResult.Invalid;

            LicenseKeys[keyId].Enable = true;
            OnUpdate(LicenseField.Enable, LicenseKeys[keyId]);
            return LicenseKeyActionResult.Success;
        }
        public class CreateLicenseKeyMetadata
        {
            public LicenseKeyMetadata[] keys;
            public LicenseGroup group;
            public string id;
            public long timestamp;
        }
        protected CreateLicenseKeyMetadata createLicenseKeyContent(
            string author,
            string[] products,
            int count = 1,
            AccountPermission[] permissions = null,
            string note = "",
            long activateBy = -1,
            string groupLabel = "")
        {

            var licenseArray = new LicenseKeyMetadata[count];
            string[] licenseIds = new string[count];
            long createTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string groupId = GeneralHelper.GenerateToken(LicenseHelper.GroupIdLength);
            for (int i = 0; i < count; i++)
            {
                licenseArray[i] = new LicenseKeyMetadata
                {
                    UID = GeneralHelper.GenerateToken(LicenseHelper.LicenseIdLength),
                    Enable = true,
                    Activated = false,
                    ActivatedBy = "",
                    InternalNote = note,
                    Key = LicenseHelper.GenerateLicenseKeyString(),
                    Products = products,
                    ProductsApplied = Array.Empty<string>(),
                    Permissions = permissions ?? Array.Empty<AccountPermission>(),
                    PermissionsApplied = Array.Empty<AccountPermission>(),
                    ActivateByTimestamp = activateBy,
                    CreatedTimestamp = createTimestamp,

                    CreatedBy = author,
                    GroupId = groupId
                };
                licenseIds[i] = licenseArray[i].UID;
            }

            var group = new LicenseGroup
            {
                UID = groupId,
                DisplayName = groupLabel,
                LicenseIds = licenseIds,
                CreatedTimestamp = createTimestamp,
                CreatedBy = author
            };
            group.manager = this;


            return new CreateLicenseKeyMetadata
            {
                group = group,
                keys = licenseArray,
                id = groupId,
                timestamp = createTimestamp
            };
        }

        public virtual async Task<CreateLicenseKeyResponse> CreateLicenseKeys(
            string author,
            string[] products,
            int count = 1,
            AccountPermission[] permissions = null,
            string note = "",
            long activateBy = -1,
            string groupLabel = "")
        {
            var createdKeys = createLicenseKeyContent(author, products, count, permissions, note, activateBy, groupLabel);

            LicenseGroups.Add(createdKeys.id, createdKeys.group);
            foreach (var item in createdKeys.keys)
            {
                LicenseKeys.Add(item.UID, item);
                OnUpdate(LicenseField.License, item);
            }
            OnUpdate(LicenseField.All, null);

            return new CreateLicenseKeyResponse
            {
                Keys = createdKeys.keys,
                GroupId = createdKeys.id
            };
        }

        public virtual async Task<GrantLicenseKeyResponseCode> GrantLicenseKey(string username, string licenseKey)
        {
            var licenseKeyMatch = LicenseHelper.LicenseKeyRegex.Match(licenseKey);
            if (!licenseKeyMatch.Success)
                return GrantLicenseKeyResponseCode.Invalid;

            var targetKey = await GetLicenseKey(licenseKey);
            if (targetKey == null)
                return GrantLicenseKeyResponseCode.Invalid;

            // If key is enabled and not activated, then we attempt to grant
            // the key if the account doesn't have all of the products.
            if (targetKey.Enable)
            {
                if (targetKey.Activated)
                    return GrantLicenseKeyResponseCode.AlreadyRedeemed;

                var account = AccountManager.GetAccountByUsername(username);
                if (account == null) return GrantLicenseKeyResponseCode.Invalid;

                // Create list of products granted and grant those products
                // if the account doesn't have them already.
                List<string> productsGranted = new List<string>();
                foreach (var product in targetKey.Products)
                {
                    if (!account.HasLicense(product, true, true))
                    {
                        productsGranted.Add(product);
                        account.GrantLicense(product);
                    }
                }
                if (productsGranted.Count < 1 && targetKey.Products.Length > 0)
                    return GrantLicenseKeyResponseCode.AlreadyRedeemed;

                // Create list of permissions granted and grant those permissions
                // if that account doesn't have them already.
                List<AccountPermission> permissionsGranted = new List<AccountPermission>();
                foreach (var perm in targetKey.Permissions)
                {
                    if (!account.HasPermission(perm))
                    {
                        permissionsGranted.Add(perm);
                        account.GrantPermission(perm);
                    }
                }

                long activatedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                LicenseKeys[targetKey.UID].Activated = true;
                LicenseKeys[targetKey.UID].ActivatedBy = username;
                LicenseKeys[targetKey.UID].ActivateTimestamp = activatedTimestamp;
                LicenseKeys[targetKey.UID].ProductsApplied = productsGranted.ToArray();
                LicenseKeys[targetKey.UID].PermissionsApplied = permissionsGranted.ToArray();

                OnUpdate(LicenseField.License, LicenseKeys[targetKey.UID]);
                return GrantLicenseKeyResponseCode.Granted;
            }
            else
            {
                return GrantLicenseKeyResponseCode.Invalid;
            }
        }
    }
}
