using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 图片倾斜填充
/// </summary>
public class ImageFillTilt : BaseMeshEffect
{
    [Range(0, 80)]
    [Header("填充角度")]
    [SerializeField]
    private int m_FillAngle = 30;

    [Header("起点位置")]
    [SerializeField]
    private Image.OriginVertical m_FillOrigin = Image.OriginVertical.Top;

    [Header("起点方向")]
    [SerializeField]
    private Image.OriginHorizontal m_StartOrigin = Image.OriginHorizontal.Right;

    [Range(0, 1)]
    [Header("填充进度")]
    [SerializeField]
    private float m_FillAmount = 1.0f;

    public int fillAngle
    {
        get
        {
            return m_FillAngle;
        }
        set
        {
            int val = Mathf.Clamp(value, 0, 80);
            if (m_FillAngle != val)
            {
                m_FillAngle = val;
                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }
    }

    public Image.OriginVertical fillOrigin
    {
        get
        {
            return m_FillOrigin;
        }
        set
        {
            if (m_FillOrigin != value)
            {
                m_FillOrigin = value;
                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }
    }

    public Image.OriginHorizontal startOrigin
    {
        get
        {
            return m_StartOrigin;
        }
        set
        {
            if (m_StartOrigin != value)
            {
                m_StartOrigin = value;
                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }
    }

    public float fillAmount
    {
        get
        {
            return m_FillAmount;
        }
        set
        {
            float val = Mathf.Clamp01(value);
            if (!m_FillAmount.Equals(val))
            {
                m_FillAmount = val;
                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (vh.currentVertCount == 0)
        {
            return;
        }

        UIVertex[] verts = new UIVertex[vh.currentVertCount];
        for (int i = 0; i < vh.currentVertCount; i++)
        {
            verts[i] = UIVertex.simpleVert;
            vh.PopulateUIVertex(ref verts[i], i);
        }

        float angle = Mathf.Tan(m_FillAngle * Mathf.Deg2Rad);
        float width = verts[2].position.x - verts[1].position.x;
        float height = verts[2].position.y - verts[3].position.y;
        float dist = height * angle;
        float len = width - dist;
        float widthUv = verts[2].uv0.x - verts[1].uv0.x;
        float lenUv = widthUv * (len / width);
        float distUv = widthUv - lenUv;

        float startX = verts[1].position.x;
        float startUv = verts[1].uv0.x;
        float fill = len * m_FillAmount;
        float fillUv = lenUv * m_FillAmount;

        if (m_StartOrigin == Image.OriginHorizontal.Right)
        {
            float longX = startX + fill + dist;
            float shortX = startX + fill;
            float longUv = startUv + fillUv + distUv;
            float shortUv = startUv + fillUv;

            int longIdx = 2;
            int shortIdx = 3;

            if (m_FillOrigin == Image.OriginVertical.Top)
            {}
            else
            {
                longIdx = 3;
                shortIdx = 2;
            }

            verts[longIdx].position.x = longX;
            verts[shortIdx].position.x = shortX;
            verts[longIdx].uv0.x = longUv;
            verts[shortIdx].uv0.x = shortUv;
        }
        else
        {
            startX = verts[3].position.x;
            startUv = verts[3].uv0.x;

            float longX = startX - fill - dist;
            float shortX = startX - fill;
            float longUv = startUv - fillUv - distUv;
            float shortUv = startUv - fillUv;

            int longIdx = 1;
            int shortIdx = 0;

            if (m_FillOrigin == Image.OriginVertical.Top)
            {
            }
            else
            {
                longIdx = 0;
                shortIdx = 1;
            }

            verts[longIdx].position.x = longX;
            verts[shortIdx].position.x = shortX;
            verts[longIdx].uv0.x = longUv;
            verts[shortIdx].uv0.x = shortUv;
        }

        vh.Clear();
        vh.AddUIVertexQuad(verts);
    }
}
