# my.Ftp

DESCRIPTION
This library allows to access an ftp server in the same programming fashion as accessing a Hard drive using the System.IO namespace.

UNIT TESTING

- Required software
An ftp server, in our case we have chosen a n open source one, such is FileZilla Server 

- Required steps
     1.Install the ftp server
     2.Create a user, with the following credentials
         a.Username: anonymous
         b.Password: anonymous
     3.Share the folder “C:\temp”, and give full permissions to the user at step 2
     4.Give full permissions to the user
     5.Start the ftp server
Once these steps are done, the unit test must work. 

