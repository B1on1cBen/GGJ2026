using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Suspect : MonoBehaviour
{
    [SerializeField] private Image bodySprite;
    [SerializeField] private GameObject dance;

    private static List<Sprite> bodySprites = new List<Sprite>();
    private static bool bodiesLoaded;

    private static void EnsureBodiesLoaded()
    {
        if (bodiesLoaded) return;
        bodySprites = new List<Sprite>(Resources.LoadAll<Sprite>("Faces/Bodies"));
        bodiesLoaded = true;
    }

    void Awake()
    {
        EnsureBodiesLoaded();
    }

    public void Dance()
    {
        bodySprite.gameObject.SetActive(false);
        dance.SetActive(true);
    }

    public void StopDance()
    {
        dance.SetActive(false);
        bodySprite.gameObject.SetActive(true);
    }

    public void GenerateSuspect(int? seed)
    {
        EnsureBodiesLoaded();

        if (bodySprites.Count > 0 && bodySprite != null)
        {
            bodySprite.sprite = bodySprites[Random.Range(0, bodySprites.Count)];
        }

        var faceManager = GetComponentInChildren<FaceManager>();
        if (faceManager != null)
        {
            faceManager.GenerateFace(seed);
        }
    }
}
