[![HitCount](http://hits.dwyl.com/parthnan/NetworkTrafficAnalysis-Csharp.svg)](http://hits.dwyl.com/parthnan/NetworkTrafficAnalysis-Csharp)
# Modeling Networks for Traffic Analysis 
An Object Oriented Approach to evaluating average waiting times for users of in Communication Networks, using basic Queuing theory(read:Kendall's Notation). Used Student's t-test for confidence interval analysis of estimated waiting time(explained below). 

Used Visual Studio .NET Core environment to build .NET Apps that do the following: 

EventDrivenSimulation v1.1 : Defines a type of event based on Arrival time and sender and places it in a FCFS queue, to be processed by one server(seearch M/D/1 queue for more details).

EventDrivenSimulation v1.2 : Extends the functionality of v1.1 by introducing M/M/1/K queue, meaning it is the same single server, but with maximum queued tasks limit of K (and a more random pattern of event arrivals called M)

EventDrivenSimulation v1.3 : Is v1.2 with K servers(M/M/K/K queue). Each server runs parallely.

EventDrivenSimulation v2.0 : Introduces M/D/1 queuing with Priority(VIP) events as well as LIFO M/M/K/K queueing.

# Challenges Overcame
Learned a lot about Network theory and Network Performance by simulating basic Networks. The challenge during coding was making sure the correct network events were picked up at correct times, while adapting to OOP in C#. The results were in accordance with basic queuing theory.
