using System.Collections.Generic;
using UnityEngine;

public class Suspect : MonoBehaviour
{
    [SerializeField] private SpriteRenderer bodySprite;

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
        GenerateSuspect(null);
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
