CREATE TABLE "User" (
	`UserId`	integer NOT NULL PRIMARY KEY AUTOINCREMENT,
	`Username`	varchar,
	`Nickname`	TEXT,
	`Password`	varchar,
	`Salt`	varchar
);
CREATE UNIQUE INDEX "User_Username" on "User"("Username")



;
CREATE TABLE "Friend"(
"UserId" integer ,
"FriendUserId" integer );
