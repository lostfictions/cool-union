using UnityEngine;
using System;

[Serializable]
public class Pair<T, U> : IEquatable<Pair<T, U>>
{
	public T First { get; private set; }
	public U Second { get; private set; }

	public Pair(T first, U second)
	{
		First = first;
		Second = second;
	}

	public bool Equals(Pair<T,U> other)
	{
        // ReSharper disable CompareNonConstrainedGenericWithNull
		return
			(((First == null) && (other.First == null))
				|| ((First != null) && First.Equals(other.First)))
			  &&
			(((Second == null) && (other.Second == null))
				|| ((Second != null) && Second.Equals(other.Second)));
        // ReSharper restore CompareNonConstrainedGenericWithNull
	}

	public override bool Equals(System.Object obj)
	{
		if (obj == null) 
			return false;

		var other = obj as Pair<T, U>;
		if (other == null)
			return false;
	
		return Equals(other);   
	}

	public override int GetHashCode()
	{
        // ReSharper disable CompareNonConstrainedGenericWithNull
		int hashcode = 0;
		if (First != null)
			hashcode += First.GetHashCode();
		if (Second != null)
			hashcode += Second.GetHashCode();
        // ReSharper restore CompareNonConstrainedGenericWithNull

		return hashcode;
	}
}
