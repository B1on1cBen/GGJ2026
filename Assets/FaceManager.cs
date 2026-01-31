using System.Collections.Generic;
using UnityEngine;

public class FaceManager : MonoBehaviour
{
    [Header("Loaded Face Parts")]
    [SerializeField] private static List<Sprite> heads = new List<Sprite>();
    [SerializeField] private static List<Sprite> eyes = new List<Sprite>();
    [SerializeField] private static List<Sprite> mouths = new List<Sprite>();
    [SerializeField] private static List<Sprite> ears = new List<Sprite>();
    [SerializeField] private static List<Sprite> hair = new List<Sprite>();
    [SerializeField] private static List<Sprite> accessories = new List<Sprite>();
    [SerializeField] private static List<Sprite> facialHair = new List<Sprite>();
    [SerializeField] private static List<Sprite> eyebrows = new List<Sprite>();
    [SerializeField] private static List<Sprite> noses = new List<Sprite>();

    [Header("Selected Face Parts")]
    [SerializeField] private Sprite selectedHead;
    [SerializeField] private Sprite selectedEyes;
    [SerializeField] private Sprite selectedMouth;
    [SerializeField] private Sprite selectedEars;
    [SerializeField] private Sprite selectedHair;
    [SerializeField] private Sprite selectedAccessory;
    [SerializeField] private Sprite selectedFacialHair;
    [SerializeField] private Sprite selectedEyebrows;
    [SerializeField] private Sprite selectedNose;

    [Header("Face Part Renderers")]
    [SerializeField] private SpriteRenderer headRenderer;
    [SerializeField] private SpriteRenderer eyesRenderer;
    [SerializeField] private SpriteRenderer mouthRenderer;
    [SerializeField] private SpriteRenderer earsRenderer;
    [SerializeField] private SpriteRenderer hairRenderer;
    [SerializeField] private SpriteRenderer accessoryRenderer;
    [SerializeField] private SpriteRenderer facialHairRenderer;
    [SerializeField] private SpriteRenderer eyebrowsRenderer;
    [SerializeField] private SpriteRenderer noseRenderer;

    [Header("Face Seed")]
    [SerializeField] private int faceSeed;
    public int FaceSeed => faceSeed;

    void Awake()
    {
        heads = new List<Sprite>(Resources.LoadAll<Sprite>("Faces/Head"));
        eyes = new List<Sprite>(Resources.LoadAll<Sprite>("Faces/Eyes"));
        mouths = new List<Sprite>(Resources.LoadAll<Sprite>("Faces/Mouth"));
        ears = new List<Sprite>(Resources.LoadAll<Sprite>("Faces/Ears"));
        hair = new List<Sprite>(Resources.LoadAll<Sprite>("Faces/Hair"));
        accessories = new List<Sprite>(Resources.LoadAll<Sprite>("Faces/Accessories"));
        facialHair = new List<Sprite>(Resources.LoadAll<Sprite>("Faces/FacialHair"));
        eyebrows = new List<Sprite>(Resources.LoadAll<Sprite>("Faces/Eyebrows"));
        noses = new List<Sprite>(Resources.LoadAll<Sprite>("Faces/Nose"));
    }

    public void GenerateFace()
    {
        GenerateFace(null);
    }

    public void GenerateFace(int? seed)
    {
        var previousState = Random.state;
        if (seed.HasValue)
        {
            faceSeed = seed.Value;
            Random.InitState(faceSeed);
        }
        else
        {
            faceSeed = Random.Range(int.MinValue, int.MaxValue);
        }

        selectedHead = heads.Count > 0 ? heads[Random.Range(0, heads.Count)] : null;
        selectedEyes = eyes.Count > 0 ? eyes[Random.Range(0, eyes.Count)] : null;
        selectedMouth = mouths.Count > 0 ? mouths[Random.Range(0, mouths.Count)] : null;
        selectedEars = ears.Count > 0 ? ears[Random.Range(0, ears.Count)] : null;
        selectedHair = hair.Count > 0 ? hair[Random.Range(0, hair.Count)] : null;
        selectedAccessory = accessories.Count > 0 ? accessories[Random.Range(0, accessories.Count)] : null;
        selectedFacialHair = facialHair.Count > 0 ? facialHair[Random.Range(0, facialHair.Count)] : null;
        selectedEyebrows = eyebrows.Count > 0 ? eyebrows[Random.Range(0, eyebrows.Count)] : null;
        selectedNose = noses.Count > 0 ? noses[Random.Range(0, noses.Count)] : null;

        headRenderer.sprite = selectedHead;
        eyesRenderer.sprite = selectedEyes;
        mouthRenderer.sprite = selectedMouth;
        earsRenderer.sprite = selectedEars;
        hairRenderer.sprite = selectedHair;
        accessoryRenderer.sprite = selectedAccessory;
        facialHairRenderer.sprite = selectedFacialHair;
        eyebrowsRenderer.sprite = selectedEyebrows;
        noseRenderer.sprite = selectedNose;

        Random.state = previousState;
    }
}
