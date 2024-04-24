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

   
