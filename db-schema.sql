CREATE TABLE "User"(
"UserId" integer primary key autoincrement not null ,
"Username" varchar ,
"Password" varchar ,
"Salt" varchar );
CREATE UNIQUE INDEX "User_Username" on "User"("Username");
