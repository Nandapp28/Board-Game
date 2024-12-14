using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SemesterConfiguration
{
    public Vector3 PlacementPosition;
    public Vector3 PlacementRotation = new Vector3(-90, -90, 8.283f);
    public GameObject SemesterPrefab;
}

[System.Serializable]
public class AllSectors
{
    public List<GameObject> Sector1;
    public List<GameObject> Sector2;
    public List<GameObject> Sector3;
    public List<GameObject> Sector4;
}

[System.Serializable]
public class SemesterCollection
{
    public SemesterConfiguration FirstSemester;
    public SemesterConfiguration SecondSemester;
    public SemesterConfiguration ThirdSemester;
    public SemesterConfiguration FourthSemester;
}

public class SemesterManager : MonoBehaviour
{
    [SerializeField] private GameObject sectorParentContainer;
    [SerializeField] private GameObject indicatorParentPrefab;
    [SerializeField] private SemesterCollection semesterConfigurations;

    private List<GameObject> sectorParents = new List<GameObject>();
    private List<GameObject> availableIndicators = new List<GameObject>();
    public int CurrentSemesterIndex = 0;
    public AllSectors NewSectors;

    private void Start()
    {
        ValidateRequiredReferences();
        InitializeSectorParents();
        InitializeIndicators();
        DistributeSemesterIndicators();
        AnimateSemesters();
    }

    private void ValidateRequiredReferences()
    {
        if (sectorParentContainer == null)
            Debug.LogError("Sector parent container is missing.");

        if (indicatorParentPrefab == null)
            Debug.LogError("Indicator parent prefab is missing.");
    }

    private void InitializeSectorParents()
    {
        sectorParents.Clear();

        foreach (Transform sectorTransform in sectorParentContainer.transform)
        {
            if (sectorTransform != null)
            {
                sectorParents.Add(sectorTransform.gameObject);
                ClearChildObjects(sectorTransform);
            }
        }

        if (sectorParents.Count == 0)
            Debug.LogWarning("No sector parents found.");
    }

    private void InitializeIndicators()
    {
        availableIndicators.Clear();

        foreach (Transform indicatorTransform in indicatorParentPrefab.transform)
        {
            if (indicatorTransform != null)
                availableIndicators.Add(indicatorTransform.gameObject);
        }

        if (availableIndicators.Count == 0)
            Debug.LogWarning("No indicators found.");
    }

    private void ClearChildObjects(Transform parentTransform)
    {
        foreach (Transform childTransform in parentTransform)
        {
            Destroy(childTransform.gameObject);
        }
    }

    private void DistributeSemesterIndicators()
    {
        if (sectorParents.Count == 0 || availableIndicators.Count == 0)
        {
            Debug.LogWarning("Insufficient sectors or indicators to distribute.");
            return;
        }

        List<List<GameObject>> sectors = new List<List<GameObject>>
        {
            NewSectors.Sector1,
            NewSectors.Sector2,
            NewSectors.Sector3,
            NewSectors.Sector4,
        };
        int index = 0 ;
        foreach (GameObject sectorParent in sectorParents)
        {
   
            PlaceSemesterIndicator(semesterConfigurations.FirstSemester, sectorParent, sectors[index]);
            PlaceSemesterIndicator(semesterConfigurations.SecondSemester, sectorParent, sectors[index]);
            PlaceSemesterIndicator(semesterConfigurations.ThirdSemester, sectorParent, sectors[index]);
            PlaceSemesterIndicator(semesterConfigurations.FourthSemester, sectorParent, sectors[index]);
            index++;
        }
    }

    private void PlaceSemesterIndicator(SemesterConfiguration semesterConfig, GameObject parentSector ,List<GameObject> sectorList)
    {
        if (semesterConfig == null)
        {
            Debug.LogError("Semester configuration is null.");
            return;
        }

        GameObject selectedIndicator = GetRandomIndicator();
        if (selectedIndicator == null)
        {
            Debug.LogWarning("No indicators available.");
            return;
        }

        CreateSemesterIndicator(selectedIndicator, semesterConfig, parentSector, sectorList);
    }

    private GameObject GetRandomIndicator()
    {
        return availableIndicators.Count > 0 
            ? availableIndicators[Random.Range(0, availableIndicators.Count)] 
            : null;
    }

    private void CreateSemesterIndicator(GameObject indicator, SemesterConfiguration semesterConfig, GameObject parentSector ,List<GameObject> sectorList)
    {
        GameObject instantiatedIndicator = Instantiate(indicator, parentSector.transform);
        instantiatedIndicator.transform.localPosition = semesterConfig.PlacementPosition;
        instantiatedIndicator.transform.localRotation = Quaternion.Euler(semesterConfig.PlacementRotation);
        
        // Simpan referensi ke objek yang di-instantiate
        
        semesterConfig.SemesterPrefab = instantiatedIndicator;
        sectorList.Add(instantiatedIndicator);
    }

    public void AnimateSemesters()
    {
        // Pastikan CurrentSemesterIndex berada dalam rentang yang valid
        if (CurrentSemesterIndex < 0 || CurrentSemesterIndex >= 4)
        {
            Debug.LogError("CurrentSemesterIndex is out of range.");
            return;
        }
        List<SemesterConfiguration> semesterConfigsToAnimate = new List<SemesterConfiguration>
        {
            semesterConfigurations.FirstSemester,
            semesterConfigurations.SecondSemester,
            semesterConfigurations.ThirdSemester,
            semesterConfigurations.FourthSemester
        };
        List<List<GameObject>> sectors = new List<List<GameObject>>
        {
            NewSectors.Sector1,
            NewSectors.Sector2,
            NewSectors.Sector3,
            NewSectors.Sector4,
        };

        for (int i = 0; i < sectors.Count; i++)
        {
            List<GameObject> Parent = sectors[i];
            Vector3 targetPostiton = semesterConfigsToAnimate[CurrentSemesterIndex].PlacementPosition;
            Vector3 targetRotation = new Vector3(90, semesterConfigsToAnimate[CurrentSemesterIndex].PlacementRotation.y, semesterConfigsToAnimate[CurrentSemesterIndex].PlacementRotation.z);
            StartCoroutine(AnimateSemesterIndicator(Parent[CurrentSemesterIndex], targetPostiton, targetRotation, 1f));
        }
    }


    private IEnumerator AnimateSemesterIndicator(GameObject semesterPrefab, Vector3 targetPosition, Vector3 targetRotation, float duration)
    {
        // Cek apakah semesterPrefab masih ada
        if (semesterPrefab == null)
        {
            Debug.LogWarning("Semester prefab has been destroyed before animation could complete.");
            yield break; // Keluar dari coroutine jika prefab sudah dihancurkan
        }

        Vector3 startPosition = semesterPrefab.transform.localPosition;
        Quaternion startRotation = semesterPrefab.transform.localRotation;
        Quaternion targetQuaternion = Quaternion.Euler(targetRotation);
        
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Cek lagi apakah semesterPrefab masih ada di dalam loop
            if (semesterPrefab == null)
            {
                Debug.LogWarning("Semester prefab has been destroyed during animation.");
                yield break; // Keluar dari coroutine jika prefab sudah dihancurkan
            }

            float t = elapsedTime / duration;
            semesterPrefab.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            semesterPrefab.transform.localRotation = Quaternion.Slerp(startRotation, targetQuaternion, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Pastikan posisi dan rotasi akhir diatur
        if (semesterPrefab != null) // Cek lagi sebelum mengatur posisi dan rotasi akhir
        {
            semesterPrefab.transform.localPosition = targetPosition;
            semesterPrefab.transform.localRotation = targetQuaternion;
        }
    }
}