using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    #region Serialized Fields
    [SerializeField] private GameObject sectorParentContainer; // Kontainer untuk sektor
    [SerializeField] private GameObject indicatorParentPrefab; // Prefab untuk indikator
    [SerializeField] private SemesterCollection semesterConfigurations; // Konfigurasi semester
    [HideInInspector] public bool IsSemesterAnimateDone = false; // Status animasi semester
    #endregion

    #region Private Variables
    private List<GameObject> sectorParents = new List<GameObject>(); // Daftar sektor
    private List<GameObject> availableIndicators = new List<GameObject>(); // Daftar indikator yang tersedia
    public int CurrentSemesterIndex = 0; // Indeks semester saat ini
    public AllSectors NewSectors; // Semua sektor
    #endregion

    #region Unity Methods
    // Memulai proses semester dengan memvalidasi referensi yang diperlukan, 
    // menginisialisasi sektor, menginisialisasi indikator, mendistribusikan indikator semester, 
    // dan memulai animasi semester.
    public void StartSemesters()
    {
        ValidateRequiredReferences();
        InitializeSectorParents();
        InitializeIndicators();
        DistributeSemesterIndicators();
        AnimateSemesters();
    }
    #endregion

    #region Initialization Methods
    // Memeriksa apakah semua referensi yang diperlukan telah diatur, 
    // dan mencetak pesan kesalahan jika ada yang hilang.
    private void ValidateRequiredReferences()
    {
        if (sectorParentContainer == null)
            Debug.LogError("Sector parent container is missing.");

        if (indicatorParentPrefab == null)
            Debug.LogError("Indicator parent prefab is missing.");
    }

    // Menginisialisasi daftar sektor dengan mengosongkan daftar yang ada 
    // dan menambahkan semua sektor yang ditemukan dalam kontainer sektor.
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

    // Menginisialisasi daftar indikator dengan mengosongkan daftar yang ada 
    // dan menambahkan semua indikator yang ditemukan dalam prefab indikator.
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

    // Menghapus semua objek anak dari transformasi induk yang diberikan.
    private void ClearChildObjects(Transform parentTransform)
    {
        foreach (Transform childTransform in parentTransform)
        {
            Destroy(childTransform.gameObject);
        }
    }
    #endregion

    #region Indicator Distribution Methods
    // Mendistribusikan indikator semester ke sektor yang tersedia 
    // berdasarkan konfigurasi semester yang telah ditentukan.
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
        int index = 0;
        foreach (GameObject sectorParent in sectorParents)
        {
            PlaceSemesterIndicator(semesterConfigurations.FirstSemester, sectorParent, sectors[index]);
            PlaceSemesterIndicator(semesterConfigurations.SecondSemester, sectorParent, sectors[index]);
            PlaceSemesterIndicator(semesterConfigurations.ThirdSemester, sectorParent, sectors[index]);
            PlaceSemesterIndicator(semesterConfigurations.FourthSemester, sectorParent, sectors[index]);
            index++;
        }
    }

    // Menempatkan indikator semester berdasarkan konfigurasi semester 
    // dan sektor yang diberikan.
    private void PlaceSemesterIndicator(SemesterConfiguration semesterConfig, GameObject parentSector, List<GameObject> sectorList)
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

    // Mengambil indikator acak dari daftar indikator yang tersedia.
    private GameObject GetRandomIndicator()
    {
        return availableIndicators.Count > 0 
            ? availableIndicators[Random.Range(0, availableIndicators.Count)] 
            : null;
    }

    // Membuat indikator semester baru berdasarkan indikator yang dipilih 
    // dan konfigurasi semester, serta menambahkannya ke daftar sektor.
    private void CreateSemesterIndicator(GameObject indicator, SemesterConfiguration semesterConfig, GameObject parentSector, List<GameObject> sectorList)
    {
        GameObject instantiatedIndicator = Instantiate(indicator, parentSector.transform);
        instantiatedIndicator.transform.localPosition = semesterConfig.PlacementPosition;
        instantiatedIndicator.transform.localRotation = Quaternion.Euler(semesterConfig.PlacementRotation);
        
        // Simpan referensi ke objek yang diinstansiasi
        semesterConfig.SemesterPrefab = instantiatedIndicator;
        sectorList.Add(instantiatedIndicator);
    }
    #endregion

    #region Animation Methods
    // Memulai proses animasi untuk semester.
    public void AnimateSemesters()
    {
        StartCoroutine(AnimateSemestersCoroutine());
    }

    // Coroutine untuk mengatur dan menjalankan animasi semester 
    // berdasarkan indeks semester saat ini.
    private IEnumerator AnimateSemestersCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        // Pastikan CurrentSemesterIndex berada dalam rentang yang valid
        if (CurrentSemesterIndex < 0 || CurrentSemesterIndex >= 4)
        {
            Debug.LogError("CurrentSemesterIndex is out of range.");
            yield break;
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
            Vector3 targetPosition = semesterConfigsToAnimate[CurrentSemesterIndex].PlacementPosition + new Vector3(0, 1, 0);
            Vector3 targetRotation = new Vector3(0, semesterConfigsToAnimate[CurrentSemesterIndex].PlacementRotation.y, semesterConfigsToAnimate[CurrentSemesterIndex].PlacementRotation.z);

            Vector3 targetPosition2 = semesterConfigsToAnimate[CurrentSemesterIndex].PlacementPosition;
            Vector3 targetRotation2 = new Vector3(90, semesterConfigsToAnimate[CurrentSemesterIndex].PlacementRotation.y, semesterConfigsToAnimate[CurrentSemesterIndex].PlacementRotation.z);
            
            // Menunggu animasi pertama selesai sebelum memulai animasi kedua
            yield return StartCoroutine(AnimateSemesterIndicator(Parent[CurrentSemesterIndex], targetPosition, targetRotation, 0.5f));
            yield return StartCoroutine(AnimateSemesterIndicator(Parent[CurrentSemesterIndex], targetPosition2, targetRotation2, 0.5f));
        }

        yield return new WaitForSeconds(1);

        IsSemesterAnimateDone = true;
    }

    // Mengatur animasi posisi dan rotasi untuk indikator semester.
    private IEnumerator AnimateSemesterIndicator(GameObject semesterPrefab, Vector3 targetPosition, Vector3 targetRotation, float duration)
    {
        if (semesterPrefab == null)
        {
            Debug.LogWarning("Semester prefab has been destroyed before animation could complete.");
            yield break;
        }

        // Menggunakan DOTween untuk animasi posisi
        semesterPrefab.transform.DOLocalMove(targetPosition, duration).SetEase(Ease.OutSine);

        // Menggunakan DOTween untuk animasi rotasi
        Quaternion targetQuaternion = Quaternion.Euler(targetRotation);
        semesterPrefab.transform.DOLocalRotate(targetQuaternion.eulerAngles, duration).SetEase(Ease.OutSine);

        // Menunggu hingga animasi selesai
        yield return new WaitForSeconds(duration);
    }
    #endregion

    // Mengubah indeks semester saat ini untuk melanjutkan ke semester berikutnya.
    public void NextSemester()
    {
        CurrentSemesterIndex++;
    }
}