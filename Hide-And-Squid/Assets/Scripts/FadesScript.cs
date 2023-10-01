using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class FadesScript : MonoBehaviour
{
    public UnityEngine.UI.Image fade;
    float fades = 1.0f;
    float time = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (fades > 0.0f && time >= 0.1f)
        {
            fades -= 0.1f;
            fade.color = new Color(0, 0, 0, fades);
            time = 0;
        }
        else if(fades <= 0.0f)
        {
            time = 0;
        }
    }
}
