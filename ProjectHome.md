Aim of this project is create an assembly to manage the OLPC (aka XO device) using C# code.
The instruction about how create a new "activity" is placed on OLPC wiki at this page: http://wiki.laptop.org/go/Mono
Here you can find the source code and the binary of Sugar-sharp, the assembly that allow to you to interact with XO device.

Finally all the source code it is released.

Now the datastore can be handled by the Datastore class that is like the Datastore python class present on XO.
Unfortunally NDesk.DBus need to be patched to use this class  .
On the download section you can find the NDesk.DBus patch to use datastore.

Finally in the Window class it is added a method to get a screeshot. This screenshot can be saved on the datastore.