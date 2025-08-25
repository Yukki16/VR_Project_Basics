using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Target : MonoBehaviour
{
    int[] dartboardNumbers = { 20, 1, 18, 4, 13, 6, 10, 15, 2, 17,
                           3, 19, 7, 16, 8, 11, 14, 9, 12, 5 }; //Fixed order

    [Header("Radius of the board")]
    [SerializeField] float bullRadius = 0.2f;
    [SerializeField] float outerBullRadius = 0.2f;

    [SerializeField] float tripleRingInner = 0.2f;
    [SerializeField] float tripleRingOuter = 0.2f;

    [SerializeField] float doubleRingInner = 0.2f;
    [SerializeField] float doubleRingOuter = 0.2f;

    [Header("UI")]
    [SerializeField] TMP_Text scoreText;

    int score = 301;
    Coroutine c = null;

    List<GameObject> dartsHit = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Tags.T_Dart))
        {
            Vector2 hitPos = new Vector2(other.transform.position.x - this.transform.position.x,
                             other.transform.position.y - this.transform.position.y);

            float distance = hitPos.magnitude; // Distance from center to determinate the ring it hit
            float angle = Mathf.Atan2(hitPos.y, hitPos.x) * Mathf.Rad2Deg; // Angle to calculate the number it hit

            angle = (-angle + 360f + 99f) % 360f; // 99 deg offset so 20 (score) is between 0 and 18 deg now, 9 deg being exactly split vertically
            int sector = Mathf.FloorToInt(angle / 18f); // Divide the angle to 18 since we have 20 numbers and the target is a circle 360/20 = 18
            int baseScore = dartboardNumbers[sector]; //Found base score

            int score = 0;

            if (distance <= bullRadius) score = 50; // bullseye
            else if (distance <= outerBullRadius) score = 25;
            else if (distance <= tripleRingOuter && distance >= tripleRingInner)
                score = baseScore * 3;
            else if (distance <= doubleRingOuter && distance >= doubleRingInner)
                score = baseScore * 2;
            else if (distance < doubleRingOuter)
                score = baseScore;
            else
                score = 0; // missed the board

            Debug.Log(score);

            if(c == null)
            {
                c = StartCoroutine(Game(score));
            }

            other.transform.parent.GetComponent<XRGrabInteractable>().enabled = false;
            dartsHit.Add(other.transform.parent.gameObject);
            Destroy(other);
        }
    }

    IEnumerator Game(int pointsEarned)
    {
        score -= pointsEarned;
        
        if(score < 0)
        {
            scoreText.text = "You lose! :(";
            yield return new WaitForSeconds(2f);
            score = 301;
            foreach(var dart in dartsHit)
            {
                Destroy(dart);
            }
            dartsHit.Clear();
        }
        else if(score == 0)
        {
            scoreText.text = "You win! :D";
            yield return new WaitForSeconds(2f);
            score = 301;
            foreach (var dart in dartsHit)
            {
                Destroy(dart);
            }
            dartsHit.Clear();
        }

        scoreText.text = score.ToString();
        yield return null;
        c = null;
    }
}
