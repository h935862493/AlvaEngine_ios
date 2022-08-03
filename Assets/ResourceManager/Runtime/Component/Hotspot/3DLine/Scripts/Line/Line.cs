using UnityEngine;
using System.Collections.Generic;
namespace Huey3DLine
{
    public class Line : LineBase
    {
        public bool active = true;
        public Color LineColor;
        public Color SpotColor;
        public float LineWidth = 0.02f;
        public float SpotSize = 0.02f;
        public int LineSmoothingValue = 10;
        private State state;
        private Transform spot;
        private GameObject line;
        private List<GameObject> segmentList = new List<GameObject>();
        private Stack<List<GameObject>> lineStack = new Stack<List<GameObject>>();
        public State GetState()
        {
            return state;
        }
        public void OnBegin()
        {
            state = State.Begin;
            segmentList = new List<GameObject>();
        }
        public void OnEnd()
        {
            state = State.End;
            lineStack.Push(segmentList);
        }
        public GameObject OnCreatLine(Vector3 from, Vector3 to)
        {
            state = State.Drawing;
            line = DrawLine(from, to, LineWidth, LineColor);
            segmentList.Add(line);
            return line;
        }
        public GameObject OnCreatSpot(Vector3 point)
        {
            state = State.Drawing;
            spot = DrawSpot(point, SpotSize, SpotColor);
            segmentList.Add(spot.gameObject);
            return spot.gameObject;
        }
        public void Revoke()
        {
            if (lineStack.Count > 0)
            {
                segmentList = lineStack.Pop();
                foreach (GameObject item in segmentList)
                {
                    Destroy(item);
                }
                segmentList.Clear();
            }
        }
        public void RevokeAll()
        {
            while (lineStack.Count > 0) Revoke();
        }
        public enum State
        {
            Begin,
            Drawing,
            End
        }
    }
}


