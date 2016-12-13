using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraScreen : MonoBehaviour
{
    [SerializeField]
    private RenderTexture m_cameraRenderTex;

    private RawImage m_screenImage;
    private RenderTexture m_currentRenderTex;
    
	private void Awake ()
    {
        m_screenImage = GetComponentInChildren<RawImage>();
        SetCameraView();
	}
	
    public void SetCameraView()
    {
        m_screenImage.texture = m_cameraRenderTex;
    }

    public void SetImage(RenderTexture renderTex)
    {
        m_screenImage.texture = renderTex; ;
    }
}
