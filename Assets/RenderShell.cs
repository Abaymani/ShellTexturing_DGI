using UnityEngine;

// Ensure a MeshFilter component is present on the GameObject
[RequireComponent(typeof(MeshFilter))]
public class RenderShell : MonoBehaviour
{
    [Header("Render Shell")]
    public Material shellMaterial; // Assign your shell shader material here
    [Range(1, 200)] // Add a reasonable range for safety/performance
    public int shellCount = 10;
    public float shellStep = 0.01f;

    private Mesh mesh;
    private Matrix4x4[] matrices;
    // private bool hasTransformChangedSinceLastUpdate = true; // More robust change detection

    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogError("MeshFilter or its sharedMesh is not found on this GameObject. Disabling RenderShell.", this);
            enabled = false; // Disable this script if no mesh is available
            return;
        }
        mesh = meshFilter.sharedMesh;

        if (shellMaterial == null)
        {
            Debug.LogError("Shell Material has not been assigned in the Inspector. Disabling RenderShell.", this);
            enabled = false;
            return;
        }

        // Initialize shader properties
        UpdateShaderProperties();
        SetupMatrices();
        // transform.hasChanged = false; // Initialize hasChanged state
    }

    void Update()
    {
        // If shellCount is changed in the inspector at runtime
        if (matrices == null || matrices.Length != shellCount)
        {
            SetupMatrices(); // Re-initialize matrices array
            UpdateShaderProperties(); // Update shader if shellCount changed
        }

        // Update matrices if the transform has changed
        // For better performance, only update if transform.hasChanged is true.
        // transform.hasChanged is automatically set by Unity when transform is modified.
        if (transform.hasChanged)
        {
            for (int i = 0; i < shellCount; i++)
            {
                matrices[i] = transform.localToWorldMatrix;
            }
            transform.hasChanged = false; // Reset the flag after updating
        }
    }
    
    void UpdateShaderProperties()
    {
        if (shellMaterial != null)
        {
            shellMaterial.SetInt("_ShellCount", shellCount); // Used for potential effects in shader
            shellMaterial.SetFloat("_ShellStep", shellStep);
        }
    }

    void SetupMatrices()
    {
        matrices = new Matrix4x4[shellCount];
        for (int i = 0; i < shellCount; i++)
        {
            // Each instance uses the same transformation matrix as the parent object.
            // The vertex shader will then offset vertices based on instance ID.
            matrices[i] = transform.localToWorldMatrix;
        }
    }

    void OnRenderObject()
    {
        if (mesh == null || shellMaterial == null || matrices == null || matrices.Length == 0 || !enabled)
        {
            return; // Don't draw if something is not set up correctly or script is disabled
        }

        // GPU instancing: Draw the mesh 'shellCount' times, each with a unique instance ID.
        // The shader uses this ID to vary the vertex offset for each shell.
        Graphics.DrawMeshInstanced(mesh, 0, shellMaterial, matrices, shellCount);
    }
}