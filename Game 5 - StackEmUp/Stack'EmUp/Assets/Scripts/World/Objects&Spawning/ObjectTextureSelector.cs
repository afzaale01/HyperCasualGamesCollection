using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for selecting textures for spawned ojects
/// </summary>
public class ObjectTextureSelector : MonoBehaviour
{
    //Static lists of materials
    private static Material[] materialsCloth;
    private static Material[] materialsWood;

    //Enums for type of material to apply
    private enum MaterialType
    {
        None,
        Wood,
        Cloth
    }

    [SerializeField] private MaterialType ourMaterialType;

    // Start is called before the first frame update
    void Start()
    {
        //If we have not yet loaded the materails do so
        if (materialsCloth == null || materialsWood == null)
        {
            LoadMaterials();
        }

        ApplyMaterial(ourMaterialType);
    }

    private void ApplyMaterial(MaterialType materialType)
    {


        //Apply a material to this object if we have materials of the
        //given type
        switch (materialType)
        {
            case MaterialType.Cloth:
                {
                    if (materialsCloth != null)
                    {
                        //Load Cloth Material if we have a renderer
                        int matIndex = Random.Range(0, materialsCloth.Length);
                        Renderer[] renderers = GetComponentsInChildren<Renderer>();
                        foreach (Renderer renderObj in renderers)
                        {
                            renderObj.material = materialsCloth[matIndex];
                        }
                            
                    }
                    break;
                }
            case MaterialType.Wood:
                {
                    if (materialsWood != null)
                    {
                        //Load Cloth Material if we have a renderer
                        int matIndex = Random.Range(0, materialsCloth.Length);
                        Renderer[] renderers = GetComponentsInChildren<Renderer>();
                        foreach (Renderer renderObj in renderers)
                        {
                            renderObj.material = materialsWood[matIndex];
                        }
                    }
                    break;
                }
            case (MaterialType.None):
                //Do Nothing
                break;
            default:
                //Do Nothing
                break;
        }
    }

    /// <summary>
    /// Load all of the furnature materials
    /// </summary>
    private static void LoadMaterials()
    {
        //Load all materials
        materialsCloth = Resources.LoadAll<Material>("SpawnableMats/Cloth/");
        materialsWood = Resources.LoadAll<Material>("SpawnableMats/Wood/");
    }
}
