using UnityEngine;

public class ShellScript : MonoBehaviour
{
    public Mesh shell;
    public Shader grassShader;
    private Material grassMaterial;
    private GameObject[] shells;

    public int shellCount = 10;
    public float length = 1.0f;
    public float density = 100.0f;
    public float minLength = 0.0f;
    public float maxLength = 1.0f;
    public float thickness = 1.0f;
    public Color grassColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grassMaterial = new Material(grassShader);

        shells = new GameObject[shellCount];
        // Creates a new gameObject that is used for a layer of the shell texture
        CreateShells();
        InitMaterials();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void CreateShells(){
        for (int i = 0; i < shellCount; i++)
        {
            // Creates a new gameObject that is used for a layer of the shell texture
            shells[i] = new GameObject("Shell " + i.ToString());
            shells[i].AddComponent<MeshFilter>();
            shells[i].AddComponent<MeshRenderer>();

            // Sets the value of the componets  to the shellMesh and shellShader
            shells[i].GetComponent<MeshFilter>().mesh = shell;
            shells[i].GetComponent<MeshRenderer>().material = grassMaterial;
            shells[i].transform.SetParent(this.transform, false);
        }
    }

    void InitMaterials(){
        for (int i = 0; i < shellCount; i++)
        {
            shells[i].GetComponent<MeshRenderer>().material.SetInt("_shellCount", shellCount);
            shells[i].GetComponent<MeshRenderer>().material.SetInt("_shellIndex", i);
            shells[i].GetComponent<MeshRenderer>().material.SetFloat("_length", length);
            shells[i].GetComponent<MeshRenderer>().material.SetFloat("_density", density);
            shells[i].GetComponent<MeshRenderer>().material.SetFloat("_thickness", thickness);
            shells[i].GetComponent<MeshRenderer>().material.SetFloat("_minLength", minLength);
            shells[i].GetComponent<MeshRenderer>().material.SetFloat("_maxLength", maxLength);
            shells[i].GetComponent<MeshRenderer>().material.SetVector("_shellColor", grassColor);
        }
    }
}
