using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private FaceManager suspectPortrait;
    [SerializeField] private Suspect[] suspects;

    void Awake()
    {
        GenerateSuspectsWithPortraitSeed();
    }

    #if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            GenerateSuspectsWithPortraitSeed();
    }
    #endif

    public void GenerateSuspectsWithPortraitSeed()
    {
        if (suspectPortrait == null) return;

        var seed = Random.Range(int.MinValue, int.MaxValue);
        suspectPortrait.GenerateFace(seed);

        if (suspects == null || suspects.Length == 0) return;

        var seededIndex = Random.Range(0, suspects.Length);
        for (int i = 0; i < suspects.Length; i++)
        {
            var s = suspects[i];
            if (s == null) continue;
            s.GenerateSuspect(i == seededIndex ? seed : (int?)null);
        }
    }
}
