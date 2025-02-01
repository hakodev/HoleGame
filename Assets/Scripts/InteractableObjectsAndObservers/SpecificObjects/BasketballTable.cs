using Alteruna;
using TMPro;
using UnityEngine;

public class BasketballTable : AttributesSync
{
    public TextMeshPro scoreText;
    private int score = 0;

    [SynchronizableMethod]
    public void IncrementText()
    {
        score++;
        scoreText.text = score.ToString();
    }
}
