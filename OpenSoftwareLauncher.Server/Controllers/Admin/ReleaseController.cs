﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OSLCommon;
using OSLCommon.Authorization;
using OSLCommon.AutoUpdater;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenSoftwareLauncher.Server.Controllers.Admin
{
    [Route("admin/release")]
    [ApiController]
    public class ReleaseController : Controller
    {
        public static AccountPermission[] RequiredPermissions = new AccountPermission[]
        {
            AccountPermission.RELEASE_MANAGE
        };

        private async Task<long> DeleteFilter(FilterDefinition<ReleaseInfo> releaseFilter, FilterDefinition<PublishedRelease> publishedFilter)
        {
            var publishedCollection = MainClass.contentManager.GetPublishedCollection();
            var releaseCollection = MainClass.contentManager.GetReleaseCollection();

            long count = 0;
            if (publishedCollection != null)
                count += (await publishedCollection.DeleteManyAsync(publishedFilter)).DeletedCount;
            if (releaseCollection != null)
                count += (await releaseCollection.DeleteManyAsync(releaseFilter)).DeletedCount;

            return count;
        }

        [HttpGet("releaseInfo")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<ReleaseInfo[]>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public async Task<ActionResult> Get_ReleaseInfo(string token)
        {
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }

            var filter = Builders<ReleaseInfo>
                .Filter
                .Empty;

            IMongoCollection<ReleaseInfo>? collection = MainClass.contentManager.GetReleaseCollection();

            IAsyncCursor<ReleaseInfo>? result = null;
            if (collection != null)
                result = await collection.FindAsync(filter);
            List<ReleaseInfo> resultList = result?.ToList() ?? new List<ReleaseInfo>();

            return Json(new ObjectResponse<ReleaseInfo[]>()
            {
                Success = true,
                Data = resultList.ToArray()
            }, MainClass.serializerOptions);
        }

        [HttpDelete("signature")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<long>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public async Task<ActionResult> Signature_Delete(string token, string signature)
        {
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }


            var publishedFilter = Builders<PublishedRelease>
                .Filter
                .Where(v => v.RemoteLocation == signature);

            var releaseFilter = Builders<ReleaseInfo>
                .Filter
                .Where(v => v.remoteLocation == signature);

            long count = await DeleteFilter(releaseFilter, publishedFilter);

            return Json(new ObjectResponse<long>()
            {
                Success = true,
                Data = count
            }, MainClass.serializerOptions);
        }
        [HttpGet("signature/releaseInfo")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<ReleaseInfo[]>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public async Task<ActionResult> Signature_Get_ReleaseInfo(string token, string signature)
        {
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }


            var filter = Builders<ReleaseInfo>
                .Filter
                .Where(v => v.remoteLocation == signature);

            IMongoCollection<ReleaseInfo>? collection = MainClass.contentManager.GetReleaseCollection();

            IAsyncCursor<ReleaseInfo>? result = null;
            if (collection != null)
                result = await collection.FindAsync(filter);
            List<ReleaseInfo> resultList = result.ToList() ?? new List<ReleaseInfo>();

            return Json(new ObjectResponse<ReleaseInfo[]>()
            {
                Success = true,
                Data = resultList.ToArray()
            }, MainClass.serializerOptions);
        }
        [HttpGet("signature/publishedRelease")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<PublishedRelease[]>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public async Task<ActionResult> Signature_Get_PublishedRelease(string token, string signature)
        {
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }

            var filter = Builders<PublishedRelease>
                .Filter
                .Where(v => v.RemoteLocation == signature);

            IMongoCollection<PublishedRelease>? collection = MainClass.contentManager.GetPublishedCollection();

            IAsyncCursor<PublishedRelease>? result = null;
            if (collection != null)
                result = await collection.FindAsync(filter);
            List<PublishedRelease> resultList = result.ToList() ?? new List<PublishedRelease>();

            return Json(new ObjectResponse<PublishedRelease[]>()
            {
                Success = true,
                Data = resultList.ToArray()
            }, MainClass.serializerOptions);
        }

        [HttpDelete("commitHash")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<long>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public async Task<ActionResult> CommitHash_Delete(string token, string hash)
        {
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }

            var publishedFilter = Builders<PublishedRelease>
                .Filter
                .Where(v => v.CommitHash == hash);
            var releaseFilter = Builders<ReleaseInfo>
                .Filter
                .Where(v => v.commitHash == hash);

            long count = await DeleteFilter(releaseFilter, publishedFilter);

            return Json(new ObjectResponse<long>()
            {
                Success = true,
                Data = count
            }, MainClass.serializerOptions);
        }

        [HttpGet("commitHash/releaseInfo")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<ReleaseInfo[]>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public async Task<ActionResult> CommitHash_Get_ReleaseInfo(string token, string hash)
        {
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }

            var filter = Builders<ReleaseInfo>
                .Filter
                .Where(v => v.commitHash == hash);

            var collection = MainClass.contentManager.GetReleaseCollection();
            IAsyncCursor<ReleaseInfo>? result = null;
            if (collection != null)
                result = await collection.FindAsync(filter);
            List<ReleaseInfo> resultList = result.ToList() ?? new List<ReleaseInfo>();

            return Json(new ObjectResponse<ReleaseInfo[]>()
            {
                Success = true,
                Data = resultList.ToArray()
            }, MainClass.serializerOptions);
        }

        [HttpGet("commitHash/publishedRelease")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<PublishedRelease[]>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public async Task<ActionResult> CommitHash_Get_PublishedRelease(string token, string hash)
        {
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }


            var filter = Builders<PublishedRelease>
                .Filter
                .Where(v => v.CommitHash == hash);

            var collection = MainClass.contentManager.GetPublishedCollection();
            IAsyncCursor<PublishedRelease>? result = null;
            if (collection != null)
                result = await collection.FindAsync(filter);
            List<PublishedRelease> resultList = result.ToList() ?? new List<PublishedRelease>();

            return Json(new ObjectResponse<PublishedRelease[]>()
            {
                Success = true,
                Data = resultList.ToArray()
            }, MainClass.serializerOptions);
        }
    }
}
