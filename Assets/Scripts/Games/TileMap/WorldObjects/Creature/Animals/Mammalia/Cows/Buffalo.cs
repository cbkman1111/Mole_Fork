using DG.Tweening;
using Spine.Unity;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TileMap
{
    public class Buffalo : Mammalia
    {
        public Transform _seat;
        public WorldObject _trainer;

        public void Seat(WorldObject obj)
        {
            if(_trainer != null)
            {
                return;
            }

            _trainer = obj;

            _trainer.transform.SetParent(_seat);
            _trainer.transform.localPosition = Vector3.zero;
            //_trainer.Skel.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        public WorldObject Unseat()
        {
            if(_trainer == null)
            {
                return null;
            }

            var player = _trainer;
            player.Skel.transform.rotation = Quaternion.Euler(60, 0, 0);
            player.transform.SetParent(null);
            player.transform.position = transform.position + new Vector3(-.2f, 0, -.2f);

            _trainer = null;

            return player;
        }
    }
}
