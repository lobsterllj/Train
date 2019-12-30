using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Train.Utilities
{
    public class CircularQueue<T>
    {
        private const int DEFAULT_CAPACITY = 100 + 1;
        private int _capacity;
        private T[] _queue;
        private int _front=0, _rear = 0;

        public CircularQueue()
        {
            _capacity = DEFAULT_CAPACITY;
            _queue = new T[_capacity];
        }

        public CircularQueue(int capacity)
        {
            if (capacity <= 0)
                throw new InvalidValueException("illegal capacity :" + capacity);
            _capacity = capacity;
            _queue =  new T[_capacity];
        }

        public void Push(T item)
        {
            if (IsFull()) Pop();    //如果队列已满，则弹出最旧的一个。一般情况下应该要报错
            _queue[_rear] = item;
            _rear = (_rear+1)%_capacity;
        }
        public T Pop()
        {
            if (_front == _rear) return default(T);
            T a = _queue[_front];
            _queue[_front] = default(T);//解除引用，对于引用类型，如果不将_queue[_front] 置为null的话，
                                                        //会导致由于队列中还存在对其的引用，而使其不能被垃圾回收
            _front = (_front+1)%_capacity;
            return a;
        }

        public int Size()
        {
            return (_rear - _front + _capacity) % _capacity;
        }
        public bool IsEmpty() { return _rear == _front; }
        public bool IsFull() { return (_rear+1)%_capacity ==_front; }
        public void DecreaseToHalf()
        {
            int half = Size() / 2;
            for (int i = 0; i < half; i++) Pop();
           // _front = (_front + half) % _capacity;
        }
        public T IndexOf(int index)
        {
            if (index >= Size()) return default(T);
            return _queue[(_front + index)%_capacity];
        }
        public IEnumerator<T> GetEnumerator()
        {
            for(int i = _front; i != _rear; i = (i + 1) % _capacity)
            {
                yield return _queue[i];
            }
        }
        

    }
 
}
