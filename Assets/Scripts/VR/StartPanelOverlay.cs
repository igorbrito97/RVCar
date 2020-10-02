using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPanelOverlay : MonoBehaviour
{

    [SerializeField] OVROverlay cameraRenderOverlay;
    void Start()
    {
        Debug.Log("STAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAART");
        DebugUIBuilder.instance.AddLabel("ÍNICIO DE SESSÃO");
        DebugUIBuilder.instance.AddDivider();
        DebugUIBuilder.instance.AddLabel("Pressione o botão START para dar partida no motor");
        DebugUIBuilder.instance.AddDivider();
        DebugUIBuilder.instance.AddLabel("\n\n\nUtilize o volantem os pedais e o câmbio de marchas para dirigir");
        DebugUIBuilder.instance.AddDivider();
        DebugUIBuilder.instance.AddLabel("\n\nSTART/XBOX - Partida");
        DebugUIBuilder.instance.AddLabel("ESC - Pausar sessão");
        
        DebugUIBuilder.instance.Show();
        cameraRenderOverlay.enabled = true;
        cameraRenderOverlay.currentOverlayShape = OVROverlay.OverlayShape.Quad;
    }

    void Update()
    {
        
    }
}
