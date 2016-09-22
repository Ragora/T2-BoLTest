datablock HoverVehicleData(Grizzly) : WildcatDamageProfile
{
   spawnOffset = "0 0 1";

   floatingGravMag = 3.5;

   catagory = "BOLVehicles";
   shapeFile = "grizzly.dts";
   computeCRC = true;

   debrisShapeName = "vehicle_grav_scout_debris.dts";
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
   explosionRadius = 5.0;

   lightOnly = 1;

   maxDamage = 5;
   destroyedLevel = 5;

   isShielded = false;
   rechargeRate = 0.7;
   energyPerDamagePoint = 95; // z0dd - ZOD, 3/30/02. Bike shield is less protective. was 75
   maxEnergy = 150;
   minJetEnergy = 15;
   jetEnergyDrain = 1.3;

   // Rigid Body
   mass = 400;
   bodyFriction = 0;
   bodyRestitution = 0.5;
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

   mainThrustForce    = 30; // z0dd - ZOD, 3/30/02. Bike main thruster more powerful. was 30
   reverseThrustForce = 10;
   strafeThrustForce  = 25;
   turboFactor        = 1; // z0dd - ZOD, 3/30/02. Bike turbo thruster more powerful. was 1.5

   brakingForce = 25;
   brakingActivationSpeed = 4;

   stabLenMin = 5.5;
   stabLenMax = 6;
   stabSpringConstant  = 15;
   stabDampingConstant = 16;

   gyroDrag = 16;
   normalForce = 30;
   restorativeForce = 20;
   steeringForce = 30;
   rollForce  = 15;
   pitchForce = 7;

   dustEmitter = VehicleLiftoffDustEmitter;
   triggerDustHeight = 2.5;
   dustHeight = 1.0;
   dustTrailEmitter = TireEmitter;
   dustTrailOffset = "0.0 -1.0 0.5";
   triggerTrailHeight = 3.6;
   dustTrailFreqMod = 15.0;

   jetSound         = FlankerThrustSound;
   engineSound      = FlankerEngineSound;
   floatSound       = FlankerThrustSound;
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
   targetNameTag = 'Armored Personnel';
   targetTypeTag = 'Carrier';
   sensorData = VehiclePulseSensor;
   sensorRadius = VehiclePulseSensor.detectRadius; // z0dd - ZOD, 3/30/02. Allows sensor to be shown on CC

   checkRadius = 1.7785;
   observeParameters = "1 10 10";

   runningLight[0] = WildcatLight1;
   runningLight[1] = WildcatLight2;
   runningLight[2] = WildcatLight3;

   shieldEffectScale = "0.9375 1.125 0.6";
};
