datablock EffectProfile(BaseSeedEngineEffect)
{
   effectname = "vehicles/mpb_thrust";
   minDistance = 50.0;
   maxDistance = 100.0;
};

datablock EffectProfile(BaseSeedThrustEffect)
{
   effectname = "vehicles/mpb_boost";
   minDistance = 50.0;
   maxDistance = 100.0;
};

datablock AudioProfile(BaseSeedEngineSound)
{
   filename    = "fx/vehicles/mpb_thrust.wav";
   description = AudioDefaultLooping3d;
   preload = true;
   effect = BaseSeedEngineEffect;
};

datablock AudioProfile(BaseSeedThrustSound)
{
   filename    = "fx/vehicles/mpb_boost.wav";
   description = AudioDefaultLooping3d;
   preload = true;
   effect = BaseSeedThrustEffect;
};

datablock StaticShapeData(DeployedBaseSeed) : StaticShapeDamageProfile
{
    interactive = true;

	className = "Base";
	shapeFile = "cca_recycler_unpacked.dts";

	maxDamage      = 0.5;
	destroyedLevel = 0.5;
	disabledLevel  = 0.3;

	isShielded = true;
	energyPerDamagePoint = 240;
	maxEnergy = 50;
	rechargeRate = 0.25;

	explosion    = HandGrenadeExplosion;
	expDmgRadius = 3.0;
	expDamage    = 0.1;
	expImpulse   = 200.0;

	dynamicType = $TypeMasks::StaticShapeObjectType;
	deployedObject = true;
	cmdCategory = "Base";
	cmdIcon = CMDSensorIcon;
	cmdMiniIconName = "commander/MiniIcons/com_deploymotionsensor";
	targetNameTag = 'Deployed Base Seed';
	deployAmbientThread = true;
	debrisShapeName = "debris_generic_small.dts";
	debris = DeployableDebris;
	heatSignature = 0;
	needsPower = false;
};

datablock StaticShapeData(UndeployedBaseSeed) : StaticShapeDamageProfile
{
	className = "Base";
	shapeFile = "cca_recycler_packed.dts";

	maxDamage      = 0.5;
	destroyedLevel = 0.5;
	disabledLevel  = 0.3;

	isShielded = true;
	energyPerDamagePoint = 240;
	maxEnergy = 50;
	rechargeRate = 0.25;

	explosion    = HandGrenadeExplosion;
	expDmgRadius = 3.0;
	expDamage    = 0.1;
	expImpulse   = 200.0;

	dynamicType = $TypeMasks::StaticShapeObjectType;
	deployedObject = true;
	cmdCategory = "Base";
	cmdIcon = CMDSensorIcon;
	cmdMiniIconName = "commander/MiniIcons/com_deploymotionsensor";
	targetNameTag = 'Deployed Base Seed';
	deployAmbientThread = true;
	debrisShapeName = "debris_generic_small.dts";
	debris = DeployableDebris;
	heatSignature = 0;
	needsPower = false;
};

datablock HoverVehicleData(BaseSeed) : WildcatDamageProfile
{
   spawnOffset = "0 0 1";

   floatingGravMag = 3.5;

   catagory = "BOLVehicles";
   shapeFile = "cca_recycler_packed.dts";
   computeCRC = true;

   debrisShapeName = "cca_recycler_packed.dts";
   debris = ShapeDebris;
   renderWhenDestroyed = false;

   drag = 0.0;
   density = 0.9;

   mountPose[0] = scoutRoot;
   cameraMaxDist = 5.0;
   cameraOffset = 0.7;
   cameraLag = 0.5;
   numMountPoints = 1;
   isProtectedMountPoint[0] = true;
   explosion = VehicleExplosion;
   explosionDamage = 0.5;
   explosionRadius = 100.0;

   lightOnly = 1;

   maxDamage = 6;
   destroyedLevel = 6;

   isShielded = true;
   rechargeRate = 0.7;
   energyPerDamagePoint = 95; // z0dd - ZOD, 3/30/02. Bike shield is less protective. was 75
   maxEnergy = 150;
   minJetEnergy = 15;
   jetEnergyDrain = 1.3;

   // Rigid Body
   mass = 5000;
   bodyFriction = 0.1;
   bodyRestitution = 0;
   softImpactSpeed = 20;       // Play SoftImpact Sound
   hardImpactSpeed = 28;      // Play HardImpact Sound

   // Ground Impact Damage (uses DamageType::Ground)
   minImpactSpeed = 29;
   speedDamageScale = 0.010;

   // Object Impact Damage (uses DamageType::Impact)
   collDamageThresholdVel = 23;
   collDamageMultiplier   = 0.030;

   dragForce            = 25 / 45.0;
   vertFactor           = 0.0;
   floatingThrustFactor = 0.35;

   mainThrustForce    = 10; // z0dd - ZOD, 3/30/02. Bike main thruster more powerful. was 30
   reverseThrustForce = 2;
   strafeThrustForce  = 3;
   turboFactor        = 1.80; // z0dd - ZOD, 3/30/02. Bike turbo thruster more powerful. was 1.5

   brakingForce = 25;
   brakingActivationSpeed = 4;

   stabLenMin = 23;
   stabLenMax = 23;
   stabSpringConstant  = 10;
   stabDampingConstant = 16;

   gyroDrag = 60;
   normalForce = 30;
   restorativeForce = 100;
   steeringForce = 50;
   rollForce  = 0;
   pitchForce = 7;

   dustEmitter = VehicleLiftoffDustEmitter;
   triggerDustHeight = 20;
   dustHeight = 20.0;
   dustTrailEmitter = TireEmitter;
   dustTrailOffset = "0.0 -1.0 0.5";
   triggerTrailHeight = 20.6;
   dustTrailFreqMod = 15.0;

   jetSound         = BaseSeedThrustSound;
   engineSound      = BaseSeedEngineSound;
   floatSound       = MPBEngineSound;
   softImpactSound  = GravSoftImpactSound;
   hardImpactSound  = HardImpactSound;
   //wheelImpactSound = WheelImpactSound;

   //
   softSplashSoundVelocity = 10.0;
   mediumSplashSoundVelocity = 20.0;
   hardSplashSoundVelocity = 30.0;
   exitSplashSoundVelocity = 10.0;

   exitingWater      = VehicleExitWaterSoftSound;
   impactWaterEasy   = VehicleImpactWaterSoftSound;
   impactWaterMedium = VehicleImpactWaterSoftSound;
   impactWaterHard   = VehicleImpactWaterMediumSound;
   waterWakeSound    = VehicleWakeSoftSplashSound;

   minMountDist = 4;

   damageEmitter[0] = SmallLightDamageSmoke;
   damageEmitter[1] = SmallHeavyDamageSmoke;
   damageEmitter[2] = DamageBubbles;
   damageEmitterOffset[0] = "0.0 -1.5 0.5 ";
   damageLevelTolerance[0] = 0.3;
   damageLevelTolerance[1] = 0.7;
   numDmgEmitterAreas = 1;

   splashEmitter[0] = VehicleFoamDropletsEmitter;
   splashEmitter[1] = VehicleFoamEmitter;

   shieldImpact = VehicleShieldImpact;

   forwardJetEmitter = WildcatJetEmitter;

   cmdCategory = Tactical;
   cmdIcon = CMDHoverScoutIcon;
   cmdMiniIconName = "commander/MiniIcons/com_landscout_grey";
   targetNameTag = 'Base Seed';
   targetTypeTag = 'Resource';
   sensorData = VehiclePulseSensor;
   sensorRadius = VehiclePulseSensor.detectRadius; // z0dd - ZOD, 3/30/02. Allows sensor to be shown on CC

   checkRadius = 1.7785;
   observeParameters = "1 10 10";

   runningLight[0] = WildcatLight1;
   runningLight[1] = WildcatLight2;
   runningLight[2] = WildcatLight3;

   shieldEffectScale = "0.9375 1.125 0.6";
};
