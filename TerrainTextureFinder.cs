using UnityEngine;

//Based upon Natty Creations "Dynamic Footsteps!: For Terrains and Imported Meshes (Unity3D)", modified by Jan Dzyr for Midnight Harvest and use of FMOD

public class TerrainTextureFinder : MonoBehaviour 
{
    private string currentSurfaceLayer;
    private string surfaceName;

    public string CheckLayers(Vector3 playerPos)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 3))
        {
            if (hit.transform.GetComponent<Terrain>() != null)
            {
                Terrain t = hit.transform.GetComponent<Terrain>();
                if (currentSurfaceLayer != GetLayerName(transform.position, t))
                {
                    currentSurfaceLayer = GetLayerName(transform.position, t);

                    ChangeFootstepSurface();        //This function will 'translate' the names of textures used by graphic team, into names we actually will use further as parameter names or values depending on need
                    return surfaceName;        
                }
            }
            if (hit.transform.GetComponent<SurfaceType>() != null)
            {
                return hit.transform.GetComponent<SurfaceType>().surfaceType.soundMaterial; //this checks for the SurfaceType script containing string "soundMaterial" for what I used Scriptable Object
            }
        }

        ChangeFootstepSurface();                    //In situation when script for some reason won't find any terrain or texture, it will keep the previous value or use default one
        return surfaceName;
    }

    private string GetLayerName(Vector3 playerPos, Terrain t)
    {
        int maxIndex = 0;
        float strongest = 0;
        float[] cellMix = GetTextureMix(playerPos, t);

        for (int i = 0; i<cellMix.Length; i++)
        {
            if (cellMix[i] > strongest)
            {
                maxIndex = i;
                strongest = cellMix[i];
            }
        }

        return t.terrainData.terrainLayers[maxIndex].name;
    }

    private float[] GetTextureMix(Vector3 playerPos, Terrain t)
    {
        Vector3 tPos = t.transform.position;
        TerrainData tData = t.terrainData;

        int mapX = Mathf.RoundToInt((playerPos.x - tPos.x) / tData.size.x * tData.alphamapWidth);
        int mapZ = Mathf.RoundToInt((playerPos.z - tPos.z) / tData.size.z * tData.alphamapHeight);

        float[,,] splatMapData = tData.GetAlphamaps(mapX, mapZ, 1, 1);
        float[] cellmix = new float[splatMapData.GetUpperBound(2) + 1];

        for (int i = 0; i < cellmix.Length; i++)
        {
            cellmix[i] = splatMapData[0, 0, i];
        }

        return cellmix;
    }

    private void ChangeFootstepSurface()
    {

        switch (currentSurfaceLayer)
        {
            //cases depends on the names you use for textures and parameters
            case "T_Dirt":
                {
                    surfaceName = "Dirt";
                    break;
                }
            case "T_Grass":
                {
                    surfaceName = "Grass";
                    break;
                }
            default:
                {
                    surfaceName = "Default";
                    break;
                }
        }
    }
}
