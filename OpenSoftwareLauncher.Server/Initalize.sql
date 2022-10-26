CREATE TABLE IF NOT EXISTS Account(
	"ID"				serial PRIMARY KEY,
	"Username"			VARCHAR(255),
	"Enabled"			BOOL
);
CREATE TABLE IF NOT EXISTS AccountToken(
	"ID"				serial PRIMARY KEY,
	"Username"			VARCHAR(255),
	"Allow"				BOOL,
	"TokenHash" 		VARCHAR(255),
	"UserAgent" 		VARCHAR(1024),
	"Host"				VARCHAR(1024),
	"CreatedTimestamp" 	VARCHAR(96),
	"LastUsed"			VARCHAR(96)
);
CREATE TABLE IF NOT EXISTS AccountPermission(
	"ID"				serial PRIMARY KEY,
	"Username" 			VARCHAR(255),
	"Permission" 		INT
);
CREATE TABLE IF NOT EXISTS AccountLicense(
	"ID"				serial PRIMARY KEY,
	"Username"			VARCHAR(255),
	"Product"			VARCHAR(512)
);
CREATE TABLE IF NOT EXISTS AccountDisableReason(
	"ID"				serial PRIMARY KEY,
	"Username"			VARCHAR(255),
	"Message"			VARCHAR(4096),
	"Timestamp"			VARCHAR(96)
);