# Routing exercise Answers



Complete this document with your answers.



----

### Candidate Name: Liu Jingtai

### Date: 23 Apr 2024

-----


## A. Implement SubscriptionIndex

- A2.  Write the names of the tests you added here:

For SubscriptionIndex.RemoveSubscriptions() function : 
1. Subscriptions_Remove_MessageType_Add_Single_Remove_Single - Add a subscription with empty ContentPattern then remove it
2. Subscriptions_Remove_MessageType_Add_Multi_Remove_Single - Add multiple subscriptions with empty ContentPattern then remove one of them
3. Subscriptions_Remove_MessageType_Add_Multi_Remove_Multi - Add multiple subscriptions with empty ContentPattern then remove all of them
4. Subscriptions_Remove_MessageTypeContentPattern_Add_Single_Remove_Single - Add a subscription with ContentPattern then remove it, the ContentPattern of the removed subscription is the same as the created one
5. Subscriptions_Remove_MessageTypeContentPattern_Add_Multi_Remove_Single - Add a subscription with ContentPattern then remove it, the ContentPattern of the removed subscription is different from the created one, e.g. when added a subscription "NASDAQ.AMZN", when system requests to remove "NASDAQ.*", which represents all subscriptions those ExchangeCode = "NASDAQ", I expected the added subscription will be removed accorindly.
6. Subscriptions_Remove_MessageTypeContentPattern_Add_Multi_Remove_Single2 - Another testing for the scenaior above, but expected non-empty result, e.g. when added a subscription "*", when system requests to remove "NYSE", which represents all subscriptions those ExchangeCode = "NYSE", I expected the added subscription won't be removed and the client can continue to receive message for ExchangeCode = "NASDAQ".

For SubscriptionIndex.RemoveSubscriptionsForConsumer() function :
1. Client_Remove_Add_Single_Remove_Single - Add single subscription to Client 1 and remove all subscriptions under Client 1
2. Client_Remove_Add_Single_Remove_Single - Add multiple subscriptions to Client 1 and remove all subscriptions under Client 1
3. Client_Remove_Add_Multi_Remove_Single - Add multiple subscriptions to Client 1 and Client 2, but only remove all subscriptions under Client 1
4. Client_Remove_Add_Multi_Remove_Single - Add multiple subscriptions to Client 1 and Client 2, and remove all subscriptions under both Client 1 and Client 2


- A3.  Briefly document your approach here (max: 500 words)
First of all, I analyze the flow of the ** MessageRouter.GetConsumers()** function, understand the main logic here is to get the matched subsciprion from the subscription data stored, which is calling the **FindSubscriptions(MessageTypeId messageTypeId, MessageRoutingContent routingContent)** function, I draw a workflow diagram to show my coded logic in this function.

From the diagram, we can see that the purpose here is to keep filtering the subscription data to find the matches, and LINQ here is the most common used function.
![image](https://github.com/Steven-Liu-0914/AbcArbitrage.Homework/assets/51730159/55f097f7-58a8-4c78-bb53-2742f4146761)

So I used ANTS Profiler to check the time used for each sub-functions. From the result below can see that the LINQ function to check existing and equal takes about 
55.5% times of process, so I tried to improve the use of LINQ part, to reduce LINQ query and find a more effective way to check item contains and equal.
![image](https://github.com/Steven-Liu-0914/AbcArbitrage.Homework/assets/51730159/efb26621-d4ef-4f89-b830-b4c0676a3673)

After some research, I found that instead of using a List for result in the FindSubscriptions method,I should use HashSet which allows a faster lookup.

**Before**
![image](https://github.com/Steven-Liu-0914/AbcArbitrage.Homework/assets/51730159/14ec5ebf-7797-4193-81c8-bbb0940132ba)

**After**
![image](https://github.com/Steven-Liu-0914/AbcArbitrage.Homework/assets/51730159/8eab1c31-b0c1-4aeb-8891-4c2ed21ce1a2)

However, after the improvement of LINQ and change to use HashSet, the performance didn't get a signaificant improvement, the running time increase to 0.045 second instead.
![image](https://github.com/Steven-Liu-0914/AbcArbitrage.Homework/assets/51730159/04fc86cc-04e7-4189-8757-d899178a17a2)

So I roll it back to the previous version. However, I believe the codes which I tried to improve is clearer and more concise to read and understand, it is not a bad try.

## C. Improve SubscriptionIndex performance (Bonus)

- C1. 
  - Did you find a solution where the benchmark executes in less than 10 microseconds?
    
  - If you did, briefly explain your approach (max: 500 words): 
    



------

### Candidate survey (optional)

The questions below are here to help us improve this homework.

1. How did you find this homework? (Easy, Intermediate, Hard)
   

2. How much time did you spend on each questions?
   - A
   - B
   - C

   
