# Desenvolvimento de Aplicações Distribuídas 
Project for DAD subject - 2020-21  
GigaStore distributed server with clients and a Puppetmaster  

# Puppetmaster
Main process that controls the creation of clients and servers and can run scripts that control the system.  
  
Buttons, inputs and their function (from left to right, top to bottom on the GUI):  
- **Replication Factor** - configures the system to replicate partitions on r servers.
    - Replication Factor - number of servers to replicate partitions.  

- **New Server** - creates a server with id **Server ID**, available at **Server URL** that delays any incoming message between **Min delay** and **Max delay** milliseconds.
    - Server ID - server identifier.
    - Server URL - URL where server is available.
    - Min delay - minimum delay of messages.
    - Max delay - maximum delay of messages.  

- **Partition** - configures the system to store **Number of replicas** replicas of partition **Partition name** on the servers identified with the ids **List of server Ids**.
    - Number of replicas - number of servers to replicate the partition.
    - Partition name - the identifier of the partition.
    - List of server Ids - identifiers of the server that make up the partition, separated by the space. The count should match **Number of replicas**  

- **New Client** - creates a client identified by the string **Client username**, available at **Client URL** and that will execute the commands in the script **Script file**.
    - Client username - client identifier.
    - Client URL - URL where client is available.
    - Script file - name of file, located in the GigaClient folder, that will be ran by the newly created client.  

- **Status** - asks every node in the system to print their status.  

- **Freeze** - freezes server identified by **Server ID**.  

- **Unfreeze** - unfreezes server identified by **Server ID**.  

- **Crash** - crashes server identified by **Server ID**.  

- **Run Script** - asks the PuppetMaster to run the script selected.


### How to Run (Windows PowerShell):
Let's assume you are on the folder that contains the project.
Change to the directory of the PuppetMaster  
```
cd .\DAD\PuppetMaster\  
```
Run the program using dotnet
```
dotnet run
```
Now you are on the GUI of the Puppetmaster and you can run any commands you want by clicking the buttons.  
To run a script:
```
Click on the <Run Script> button  

Select the script you want to run (there is a sample script in the root folder of the project)  

Press <Abrir> or <Open> depending on your System's language  

The script will start running
```
You can still order new commands after running a script