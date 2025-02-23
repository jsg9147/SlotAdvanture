using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoulettePiece : MonoBehaviour
{
    [SerializeField]
    public Image icon;
    [SerializeField]
    public TextMeshProUGUI textDescription;

    public void Setup(RoulettePieceData pieceData)
    {
        icon.sprite = pieceData.icon;
        textDescription.text = pieceData.description;
    }
}
