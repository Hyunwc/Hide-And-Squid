using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Textcolor : BaseMeshEffect //BaseMeshEffect 클래스를 상속받음.
{
    Text txt;
    public Gradient myGradient; //그라디언트 생성.

    float gradientWaveTime;

    protected override void Start()
    {
        txt = GetComponent<Text>();
    }
    private void Update()
    {
        gradientWaveTime += Time.deltaTime;

        txt.FontTextureChanged();
    }
    public override void ModifyMesh(VertexHelper vh)
    {
        List<UIVertex> vertices = new List<UIVertex>();
        vh.GetUIVertexStream(vertices); //Vertex를 얻어옴.

        float min = vertices.Min(t => t.position.x);
        float max = vertices.Max(t => t.position.x);

        for (int i = 0; i < vertices.Count; i++)
        {
            var v = vertices[i];
            float curXNormalized = Mathf.InverseLerp(min, max, v.position.x); //Lerp함수의 역임.
            curXNormalized = Mathf.PingPong(curXNormalized + gradientWaveTime, 1f); //1을 넘어가지 않는선에서 왓다갓다함.
            Color c = myGradient.Evaluate(curXNormalized);

            v.color = new Color(c.r, c.g, c.b, 1); //0~255
            vertices[i] = v;
        }
        vh.Clear();
        vh.AddUIVertexTriangleStream(vertices);//vertices 등록
    }
}
