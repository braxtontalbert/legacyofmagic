using System.Collections;
using ThunderRoad;
using UnityEngine;
namespace LegacyOfMagic.Spells.SpellMonos.Updaters
{
    public class LeviosoUpdate : MonoBehaviour
    {
        private Rigidbody rigidbody;
        private Vector3 position;

        private void Start()
        {
            GameManager.local.StartCoroutine(StopLevitate(gameObject));
        }

        public void Setup(Rigidbody rigid, Vector3 pos)
        {
            this.rigidbody = rigid;
            this.position = pos;
        }

        IEnumerator StopLevitate(GameObject go)
        {
            yield return new WaitForSeconds(10f);
            Destroy(go);
        }
        private void Update()
        {
            if (rigidbody)
            {
                rigidbody.velocity = ((position + (Vector3.up * 4f)) - rigidbody.transform.position) * 5f;
            }
        }
    }
}