using UnityEngine;

public class FixedTextureSizeAnimator : GameObjectAnimator
{
    public Vector2 m_textureSize { get; set; }

    public override void OnScaleChanged(Vector3 newScale)
    {
        base.OnScaleChanged(newScale);
        
        UVQuad quad = this.gameObject.GetComponent<UVQuad>();

        Vector3 objectSize = GetGameObjectSize();
        
        Vector2 texturePivotPoint = m_pivotPoint;
        texturePivotPoint.Scale(m_textureSize);

        Vector2 topLeft = new Vector2(texturePivotPoint.x - m_pivotPoint.x * objectSize.x, texturePivotPoint.y - m_pivotPoint.y * objectSize.y);

        quad.m_textureRange = new Vector4(topLeft.x, topLeft.y, objectSize.x, objectSize.y);
        quad.m_textureRange.Scale(new Vector4(1 / m_textureSize.x, 1 / m_textureSize.y, 1 / m_textureSize.x, 1 / m_textureSize.y));
    }
}