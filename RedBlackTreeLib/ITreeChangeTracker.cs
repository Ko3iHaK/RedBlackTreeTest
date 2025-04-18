using System;

namespace RedBlackTreeLib;

public interface ITreeChangeTracker<T> where T : IComparable<T>
{
    void TrackInsert(T key);

    void TrackRotation(string direction);

    void TrackColorChange(Node<T> node);
}