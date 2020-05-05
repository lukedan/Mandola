using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : BasicGun {
	/// <summary>
	/// The number of pallets for each shot.
	/// </summary>
	public int NumPallets = 7;

	protected override void _Fire(Vector3 aim) {
		Vector3 position = transform.position;
		Vector3 direction = (aim - position).normalized;
		for (int i = 0; i < NumPallets; ++i) {
			Vector3 palletDir = direction * AccuracyDistance + UnityEngine.Random.insideUnitSphere * ShootInaccuracy;
			palletDir = palletDir.normalized;
			Bullet bullet = _CreateBullet(position + direction * FireOffset);
			bullet.Velocity = palletDir * BulletSpeed + _parentVelocity.velocity;
			bullet.Damage = BulletDamage;
            ShootAudioSource.PlayOneShot(ShootAudioClip);
        }
	}
}
