using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NewTestScript
{
    //// A Test behaves as an ordinary method
    //[Test]
    //public void NewTestScriptSimplePasses()
    //{
    //    // Use the Assert class to test conditions
    //}

    //// A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    //// `yield return null;` to skip a frame.
    //[UnityTest]
    //public IEnumerator NewTestScriptWithEnumeratorPasses()
    //{
    //    // Use the Assert class to test conditions.
    //    // Use yield to skip a frame.
    //    yield return null;
    //}


    [Test]
    public void Test_PriorityQueue_Enqueue_Dequeue()
    {
        Func<int, int, bool> compare = (a, b) => a > b;
        PriorityQueue<int> pq = new PriorityQueue<int>(100, compare);
        pq.Enqueue(3);
        pq.Enqueue(2);
        pq.Enqueue(1);
        Assert.AreEqual(3, pq.Dequeue());
        Assert.AreEqual(2, pq.Dequeue());
        Assert.AreEqual(1, pq.Dequeue());
    }
}
