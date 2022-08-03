using UnityEngine;
namespace Huey3DLine
{
    public abstract class LineBase : MonoBehaviour
    {
        public enum Quality
        {
            Height,
            Low
        }
        public enum LineType
        {
            SolidLine,
            DottedLine
        }
        public Quality LineQuality;
        public LineType lineType;
        public bool AngleSmoothing = true;
        private GameObject spotPrefab;
        private GameObject linePrefab;
        private Quality m_quality;
        public virtual void Start()
        {
            m_quality = LineQuality;
            QualityChanged(LineQuality);
            spotPrefab = Resources.Load("3DLine/spot") as GameObject;
            linePrefab = Resources.Load("3DLine/lineHeight") as GameObject;
        }
        public void QualityChanged(Quality quality)
        {
            m_quality = quality;
            switch (quality)
            {
                case Quality.Height:
                    linePrefab = Resources.Load("3DLine/lineHeight") as GameObject;
                    break;
                case Quality.Low:
                    linePrefab = Resources.Load("3DLine/lineLow") as GameObject;
                    break;
            }
        }
        public GameObject DrawLine(Vector3 pointA, Vector3 pointB, float width, Color color)
        {
            if (linePrefab == null) return null;
            if (m_quality != LineQuality) QualityChanged(LineQuality);
            float distance = Vector3.Distance(pointA, pointB);
            float scale = distance;
            if (LineQuality == Quality.Height) scale = (distance / 2);
            if (lineType == LineType.DottedLine) scale /= 2;
            Vector3 linePoint = pointA + (pointB - pointA) / 2;
            if (GameObject.Find("Lines") == null)
            {
                GameObject game = new GameObject();
                game.name = "Lines";
            }
            Transform m_line = Instantiate(linePrefab, linePoint, Quaternion.identity, GameObject.Find("Lines").transform).transform;
            m_line.GetComponent<MeshRenderer>().material.color = color;
            m_line.LookAt(pointB);
            m_line.Rotate(Vector3.left, 90, Space.Self);
            m_line.localScale = new Vector3(width, scale, width);
            return m_line.gameObject;
        }
        public GameObject DrawLine(Vector3 pointA, Vector3 pointB, float width)
        {
            /*if (m_quality != LineQuality) */QualityChanged(LineQuality);
            float distance = Vector3.Distance(pointA, pointB);
            float scale = distance;
            if (LineQuality == Quality.Height) scale = (distance / 2);
            if (lineType == LineType.DottedLine) scale /= 2;
            Vector3 linePoint = pointA + (pointB - pointA) / 2;
            if(GameObject.Find("Lines") == null)
            {
                GameObject game = new GameObject();
                game.name = "Lines";
            }
            Transform m_line = Instantiate(linePrefab, linePoint, Quaternion.identity, GameObject.Find("Lines").transform).transform;
            m_line.LookAt(pointB);
            m_line.Rotate(Vector3.left, 90, Space.Self);
            m_line.localScale = new Vector3(width, scale, width);
            return m_line.gameObject;
        }
        public Transform DrawSpot(Vector3 point, float scale, Color color)
        {
            GameObject spotObj;
            if (AngleSmoothing)
            {
                spotObj = Instantiate(spotPrefab, point, Quaternion.identity, transform);
                spotObj.GetComponent<MeshRenderer>().material.color = color;
                spotObj.transform.localScale = new Vector3(scale, scale, scale);
                return spotObj.transform;
            }
            else
            {
                return Instantiate(new GameObject(), point, Quaternion.identity, transform).transform;
            }
        }
    }
}

