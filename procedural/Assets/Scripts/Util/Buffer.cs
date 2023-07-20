using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

public struct Buffer<T> : IEnumerable<T> {

    public T[] _array;
    public int _count;

    public Buffer(int initialCapacity) {
        _array = new T[initialCapacity];
        _count = 0;
    }

    public int Count => _count;
    public ref T this[int index] => ref _array[index];

    public void EnsureCapacity(int capacity) {
        if (capacity > _array.Length) {
            var newArray = new T[NextPowerOf2(capacity)];
            for (int i = 0; i < _count; i++) {
                newArray[i] = _array[i];
            }
            _array = newArray;
        }
    }

    public void Add(T e) {
        EnsureCapacity(_count + 1);
        _array[_count] = e;
        _count++;

    }

    public void AddRange(IEnumerable<T> range) {
        foreach (var e in range) {
            Add(e);
        }
    }

    // Performance optimization
    public void AddRange(Buffer<T> buffer) {
        EnsureCapacity(Count + buffer.Count);
        for (int i = 0; i < buffer.Count; i++) {
            _array[_count++] = buffer[i];
        }
    }

    // Performance optimization
    public void AddRange(T[] array) {
        EnsureCapacity(Count + array.Length);
        for (int i = 0; i < array.Length; i++) {
            _array[_count++] = array[i];
        }
    }

    private static int NextPowerOf2(int x) {
        return x == 1 ? 1 : 1 << (32 - (math.lzcnt(x) - 1));
    }

    public IEnumerator<T> GetEnumerator() => new Enumerator(this);
    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    class Enumerator : IEnumerator<T> {
        private int _index;
        private Buffer<T> _buffer;

        public Enumerator(Buffer<T> buffer) {
            _buffer = buffer;
            _index = -1;
        }

        public T Current => _buffer[_index];
        object IEnumerator.Current => _buffer[_index];

        public void Dispose() { }
        public void Reset() => _index = -1;
        public bool MoveNext() => ++_index < _buffer.Count;

    }
}
