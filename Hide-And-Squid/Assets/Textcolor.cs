using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Textcolor : BaseMeshEffect //BaseMeshEffect Ŭ������ ��ӹ���.
{
    Text txt;
    public Gradient myGradient; //�׶���Ʈ ����.

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
        vh.GetUIVertexStream(vertices); //Vertex�� ����.

        float min = vertices.Min(t => t.position.x);
        float max = vertices.Max(t => t.position.x);

        for (int i = 0; i < vertices.Count; i++)
        {
            var v = vertices[i];
            float curXNormalized = Mathf.InverseLerp(min, max, v.position.x); //Lerp�Լ��� ����.
            curXNormalized = Mathf.PingPong(curXNormalized + gradientWaveTime, 1f); //1�� �Ѿ�� �ʴ¼����� �Ӵٰ�����.
            Color c = myGradient.Evaluate(curXNormalized);

            v.color = new Color(c.r, c.g, c.b, 1); //0~255
            vertices[i] = v;
        }
        vh.Clear();
        vh.AddUIVertexTriangleStream(vertices);//vertices ���
    }
}
