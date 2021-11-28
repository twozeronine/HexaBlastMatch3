using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3OriginDataStructure
{
    internal class PriorityQueue<T> where T : IComparable<T>
    {
        private readonly List<T> heap = new List<T>();

        // O(log2(N))
        public void Push(T data)
        {
            // 힙의 맨 끝에 새로운 데이터를 삽입한다
            heap.Add(data);

            var now = heap.Count - 1;
            // 도장깨기를 시작
            while (now > 0)
            {
                // 도장깨기를 시도
                var next = (now - 1) / 2;
                if (heap[now].CompareTo(heap[next]) < 0)
                    break; // 현재 값이 부모값보다 작으면 실패

                // 현재 값이 부모값보다 크다면
                // 두 값을 교체한다
                (heap[now], heap[next]) = (heap[next], heap[now]);

                // 검사 위치를 이동한다
                now = next;
            }
        }

        public T Pop()
        {
            // 반환할 데이터를 따로 저장
            T ret = heap[0];

            // 마지막 데이터를 루트로 이동한다
            var lastIndex = heap.Count - 1;
            heap[0] = heap[lastIndex];
            heap.RemoveAt(lastIndex);
            lastIndex--;

            // 역으로 내려가는 도장깨기 시작
            var now = 0;
            while (true)
            {
                int left = 2 * now + 1;
                int right = 2 * now + 2;

                int next = now;
                // 왼쪽값이 현재값보다 크면, 왼쪽으로 이동 
                if (left <= lastIndex && heap[next].CompareTo(heap[left]) < 0)
                    next = left;
                // 오른쪽값이 현재값(왼쪽 이동 포함)보다 크면, 오른쪽으로 이동
                if (right <= lastIndex && heap[next].CompareTo(heap[right]) < 0)
                    next = right;

                // 왼쪽/오른쪽 모두 현재값보다 작으면 종료
                if (next == now)
                    break;


                // 두 값을 교체한다
                (heap[now], heap[next]) = (heap[next], heap[now]);
                //검사 위치를 이동한다
                now = next;

            }

            return ret;
        }

        public int Count => heap.Count;

        public void Clear() => heap.Clear();
    }    
}

