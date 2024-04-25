# Routing exercise Answers



Complete this document with your answers.



----

### Candidate Name: Liu Jingtai

### Date: 25 Apr 2024

-----


## A. Implement SubscriptionIndex

- A2.  Write the names of the tests you added here:

Tests For SubscriptionIndex.RemoveSubscriptions() function : 
1. Subscriptions_Remove_MessageType_Add_Single_Remove_Single - Add a subscription with empty ContentPattern then remove it
2. Subscriptions_Remove_MessageType_Add_Multi_Remove_Single - Add multiple subscriptions with empty ContentPattern then remove one of them
3. Subscriptions_Remove_MessageType_Add_Multi_Remove_Multi - Add multiple subscriptions with empty ContentPattern then remove all of them
4. Subscriptions_Remove_MessageTypeContentPattern_Add_Single_Remove_Single - Add a subscription with ContentPattern then remove it, the ContentPattern of the removed subscription is the same as the created one
5. Subscriptions_Remove_MessageTypeContentPattern_Add_Multi_Remove_Single - Add a subscription with ContentPattern then remove it, the ContentPattern of the removed subscription is different from the created one, e.g. when added a subscription "NASDAQ.AMZN", when system requests to remove "NASDAQ.*", which represents all subscriptions those ExchangeCode = "NASDAQ", I expected the added subscription will be removed accorindly.
6. Subscriptions_Remove_MessageTypeContentPattern_Add_Multi_Remove_Single2 - Another testing for the scenaior above, but expected non-empty result, e.g. when added a subscription "*", when system requests to remove "NYSE", which represents all subscriptions those ExchangeCode = "NYSE", I expected the added subscription won't be removed and the client can continue to receive message for ExchangeCode = "NASDAQ".

Tests For SubscriptionIndex.RemoveSubscriptionsForConsumer() function :
1. Client_Remove_Add_Single_Remove_Single - Add single subscription to Client 1 and remove all subscriptions under Client 1
2. Client_Remove_Add_Single_Remove_Single - Add multiple subscriptions to Client 1 and remove all subscriptions under Client 1
3. Client_Remove_Add_Multi_Remove_Single - Add multiple subscriptions to Client 1 and Client 2, but only remove all subscriptions under Client 1
4. Client_Remove_Add_Multi_Remove_Single - Add multiple subscriptions to Client 1 and Client 2, and remove all subscriptions under both Client 1 and Client 2


- A3.  Briefly document your approach here (max: 500 words)
First of all, I analyze the flow of the ** MessageRouter.GetConsumers()** function, when coded for question A1, I understand the main logic here is to get the matched subsciprion from the subscription data stored, which is calling the **FindSubscriptions(MessageTypeId messageTypeId, MessageRoutingContent routingContent)** function, I draw a workflow diagram to show my coded logic in this function.

From the diagram, we can see that the purpose here is to keep filtering the subscription data to find the matches, and most of the time I used LINQ to check and filter the data.
![image](https://github.com/Steven-Liu-0914/AbcArbitrage.Homework/assets/51730159/55f097f7-58a8-4c78-bb53-2742f4146761)

So I used ANTS Profiler to check the time used and create a console app to call the testing method **ShouldIncludeMatchingRoutableSubscriptionWithLongPattern()** in **MessageRouterTests**. 
From the result below can see that the LINQ function to check existing and equal takes about 60% times to process, so I tried to improve the performance by enhancing the code to select data using LINQ and looking for a more effective way for data reading.
![image](https://github.com/Steven-Liu-0914/AbcArbitrage.Homework/assets/51730159/04e12bfd-5faf-4038-929a-95fc044015d5)


After some research, I found that instead of using a List for result in the FindSubscriptions method, HashSet might be a better option to allow a faster lookup, so I made changes as below :

1. Change from List<Subscription> to HashSet<Subscription>
2. Consilidate the codes to combine validation for routingContent.Parts in one condition check.
3. Use UnionWith to insert batch data instead of foreach to add one by one and remove the "contains" validation before data inserting to improve performance

**Before**
![image](https://github.com/Steven-Liu-0914/AbcArbitrage.Homework/assets/51730159/24493a10-eb3f-4937-bd3c-ccc0a5a60db9)


**After**
![image](https://github.com/Steven-Liu-0914/AbcArbitrage.Homework/assets/51730159/8eab1c31-b0c1-4aeb-8891-4c2ed21ce1a2)

After the reducing the usage of LINQ and change to use HashSet, the performance improved a bit, the process time reduced from 0.021 second to 0.018 second.
![image](https://github.com/Steven-Liu-0914/AbcArbitrage.Homework/assets/51730159/ba8bb89d-f0e1-4db1-b1fb-0383f1f065c6)



Although the performance only improved very little, I believe the new-version codes will be clearer and more concise to read and understand.

## C. Improve SubscriptionIndex performance (Bonus)

- C1. 
  - Did you find a solution where the benchmark executes in less than 10 microseconds?
  
Unfortunately I didn't find best way to improve SubscriptionIndex performance to less than 10 ms, my best result here 24.91ms for Mean Method.
![image](https://github.com/Steven-Liu-0914/AbcArbitrage.Homework/assets/51730159/d5135e23-61dc-4054-bb3c-c730e3b9a899)

How I tried to improve the performace you may refer to my answer to A3.

P.S. allow me to change the main program from BenchmarkSwitcher to BenchmarkRunner as there is only one Benchmarks
![image](https://github.com/Steven-Liu-0914/AbcArbitrage.Homework/assets/51730159/eaee600f-16d7-42d8-bed0-fcd2dae892ca)


  - If you did, briefly explain your approach (max: 500 words): 
    



------

### Candidate survey (optional)

The questions below are here to help us improve this homework.

1. How did you find this homework? (Easy, Intermediate, Hard)

   Between Intermediate and Hard

2. How much time did you spend on each questions?
   - A : A1 spent about 6 hours to be familiar with the project at the beginning; A2 about 2-3 hours; A3 about 4 hours to research and testing on performance profiler
   - B : about 3 hours, Q2 is more stright-forward comparing with Q1
   - C : about 4 hours, benchmarks is something new to me, good to learn.

   
