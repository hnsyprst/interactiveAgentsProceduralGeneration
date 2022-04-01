using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generic class
// Takes items of generic type T, but T must have implemented IHeapItem
public class Heap<T> where T : IHeapItem<T>
{
    T[] Items;
    int CurrentItemCount;

    public Heap(int MaxHeapSize)
    {
        Items = new T[MaxHeapSize];
    }

    public void Add(T Item)
    {
        // Set the passed Item's heap index to the current number of items stored on the heap
        // i.e., set it's index to the bottom of the heap
        Item.HeapIndex = CurrentItemCount;
        // Add the item to the bottom of the heap
        Items[CurrentItemCount] = Item;
        // Sort the item into its correct position
        SortUp(Item);
        CurrentItemCount++;
    }

    void SortUp(T Item)
    {
        // Mathematical trick for finding the parent of the current item
        // It is always the item's index - 1, divided by 2
        int ParentIndex = (Item.HeapIndex - 1) / 2;

        // This loop will keep swapping Item up through the heap until it finds a Parent Item with higher priority than Item
        while (true)
        {
            T ParentItem = Items[ParentIndex];

            // If the current Item has a higher priority (for example, a lower FCost in an A* implementation) than its parent, CompareTo will return 1.
            // If it has the same priority, CompareTo will return 0.
            // If it has a lower priority, CompareTo will return -1.
            if (Item.CompareTo(ParentItem) > 0)
            {
                Swap(Item, ParentItem);
            }
            else
            {
                break;
            }

            ParentIndex = (Item.HeapIndex - 1) / 2;
        }
    }

    void Swap(T ItemA, T ItemB)
    {
        Items[ItemA.HeapIndex] = ItemB;
        Items[ItemB.HeapIndex] = ItemA;

        int ItemAIndex = ItemA.HeapIndex;
        ItemA.HeapIndex = ItemB.HeapIndex;
        ItemB.HeapIndex = ItemAIndex;
    }

    public T Pop()
    {
        // Pop the first item off the heap
        T FirstItem = Items[0];
        CurrentItemCount--;

        // Move the bottom item from the heap to the top
        Items[0] = Items[CurrentItemCount];
        Items[0].HeapIndex = 0;
        // Sort the bottom item into the correct place in the heap
        SortDown(Items[0]);

        return FirstItem;
    }

    void SortDown(T Item)
    {
        // This loop will keep swapping Item down through the heap until it finds a Child Item with lower priority than Item
        // Or until it reaches the bottom of the heap
        while (true)
        {
            // Mathematical trick for finding the parent of the current item
            // It is always the item's index - 1, divided by 2
            // So the reverse is true - the children are parent's index multiplied by 2
            // And + 1 and + 2 for each child, left and right
            int ChildIndexLeft = Item.HeapIndex * 2 + 1;
            int ChildIndexRight = Item.HeapIndex * 2 + 2;

            int SwapIndex = 0;

            // If Item has a child on the left
            if (ChildIndexLeft < CurrentItemCount)
            {
                // Set the index to be compared with Item to the left child's index
                SwapIndex = ChildIndexLeft;
                
                // If Item also has a child on the right
                if (ChildIndexRight < CurrentItemCount)
                {
                    // Compare the priority of both children, and change the swap index
                    // to the right child if the right child is higher priority - 
                    // otherwise leave it as left
                    if (Items[ChildIndexLeft].CompareTo(Items[ChildIndexRight]) < 0)
                    {
                        SwapIndex = ChildIndexRight;
                    }
                }

                // If the highest priority child of Item has higher priority than Item
                // Swap these items
                if (Item.CompareTo(Items[SwapIndex]) < 0)
                {
                    Swap(Item, Items[SwapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    // This method should be called when an item's priority has been reduced
    // In A* pathfinding, this would be when the cost has been increased
    public void UpdateItem(T Item)
    {
        SortUp(Item);
    }

    public int Count
    {
        get
        {
            return CurrentItemCount;
        }
    }

    public bool Contains(T Item)
    {
        // If Items contains an item with the same Heap Index as Item
        // return true; Item is in the list
        return Equals(Items[Item.HeapIndex], Item);
    }
}


public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}
