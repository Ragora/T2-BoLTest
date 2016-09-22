//--------------------------------------------------------------------------------------------------
//  This is the primary hook for the client end. It essentially hooks into the AudioEmitter
//  ::onEditorRender to dispatch all of the draw commands submitted to our client end by the
//  remote server. This is called once per iteration per emitter in the fake clientside MissionGroup
//  which is required for this function to ever be called at all.
//
//  We use audio emitters because the only other option appears to be SpawnSpheres which require a
//  datablock to instantiate, audio emitters do not and therefore they only serve as markers for our
//  code.
//
//  This software is licensed under the MIT license. Consult LICENSE.txt for licensing details.
//  Copyright (c) 2016 Robert MacGregor
//==================================================================================================
package DrawSubsystemHooks
{
    function AudioEmitter::onEditorRender(%this, %editor, %selected, %expanded)
    {
        if (%this.isClientDrawer)
        {
            // If we're one of the special renders, loop for attached objects
            if (isObject(BoundingBoxDrawMarker) && %this == BoundingBoxDrawMarker.getID())
            {
                BoundingBoxDrawMarker.targets.iterationBegin();
                while (!BoundingBoxDrawMarker.targets.iterationEnd())
                {
                    %currentObj = BoundingBoxDrawMarker.targets.nextObject();

                    // Technically this breaks the current iteration slightly, but this is an error case
                    // and the clients shouldn't really be able to notice the one-frame drop for that one
                    // object that gets screwed on draws during this pass due to the value removal.
                    if (!isObject(%currentObj))
                    {
                        error("!!! ClientDraw (onEditorRender): Found non-existent object '" @ %current SPC "' at index " @ BoundingBoxDrawMarker.targets.getIterationIndex() SPC "during draw. Fell out of scope?");
                        BoundingBoxDrawMarker.targets.removeValue(%current);
                        continue;
                    }

                    %editor.consoleFrameColor = $ClientDrawer::BoundingBoxColor[%currentObj];
                    %editor.consoleFillColor = $ClientDrawer::BoundingBoxColor[%currentObj];

                    %worldBox = %currentObj.getWorldBox();

                    // These are diagonally across from each other
                    %lowerRearLeftPoint = getWords(%worldBox, 0, 2);
                    %upperFrontRightPoint = getWords(%worldBox, 3, 5);

                    %upperFrontRightX = getWord(%upperFrontRightPoint, 0);
                    %upperFrontRightY = getWord(%upperFrontRightPoint, 1);
                    %upperFrontRightZ = getWord(%upperFrontRightPoint, 2);
                    %lowerRearLeftX = getWord(%lowerRearLeftPoint, 0);
                    %lowerRearLeftY = getWord(%lowerRearLeftPoint, 1);
                    %lowerRearLeftZ = getWord(%lowerRearLeftPoint, 2);

                    // Calculate remaining rear points
                    %lowerRearRightPoint = %upperFrontRightX SPC %lowerRearLeftY SPC %lowerRearLeftZ;
                    %upperRearRightPoint = %upperFrontRightX SPC %lowerRearLeftY SPC %upperFrontRightZ;
                    %upperRearLeftPoint = %lowerRearLeftX SPC %lowerRearLeftY SPC %upperFrontRightZ;

                    // Calculate remaining front points
                    %upperFrontLeftPoint = %lowerRearLeftX SPC %upperFrontRightY SPC %upperFrontRightZ;
                    %lowerFrontLeftPoint = %lowerRearLeftX SPC %upperFrontRightY SPC %lowerRearLeftZ;
                    %lowerFrontRightPoint = %upperFrontRightX SPC %upperFrontRightY SPC %lowerRearLeftZ;

                    // Draw Back Plane
                    %editor.renderLine(%lowerRearLeftPoint, %lowerRearRightPoint, $ClientDrawer::BoundingBoxWidth[%currentObj]);
                    %editor.renderLine(%lowerRearLeftPoint, %upperRearLeftPoint, $ClientDrawer::BoundingBoxWidth[%currentObj]);
                    %editor.renderLine(%lowerRearRightPoint, %upperRearRightPoint, $ClientDrawer::BoundingBoxWidth[%currentObj]);
                    %editor.renderLine(%upperRearLeftPoint, %upperRearRightPoint, $ClientDrawer::BoundingBoxWidth[%currentObj]);

                    // Draw Front Plane
                    %editor.renderLine(%upperFrontRightPoint, %upperFrontLeftPoint, $ClientDrawer::BoundingBoxWidth[%currentObj]);
                    %editor.renderLine(%upperFrontRightPoint, %lowerFrontRightPoint, $ClientDrawer::BoundingBoxWidth[%currentObj]);
                    %editor.renderLine(%upperFrontLeftPoint, %lowerFrontLeftPoint, $ClientDrawer::BoundingBoxWidth[%currentObj]);
                    %editor.renderLine(%lowerFrontLeftPoint, %lowerFrontRightPoint, $ClientDrawer::BoundingBoxWidth[%currentObj]);

                    // Draw connecting planes
                    %editor.renderLine(%upperFrontRightPoint, %upperRearRightPoint, $ClientDrawer::BoundingBoxWidth[%currentObj]);
                    %editor.renderLine(%upperFrontLeftPoint, %upperRearLeftPoint, $ClientDrawer::BoundingBoxWidth[%currentObj]);
                    %editor.renderLine(%lowerFrontRightPoint, %lowerRearRightPoint, $ClientDrawer::BoundingBoxWidth[%currentObj]);
                    %editor.renderLine(%lowerFrontLeftPoint, %lowerRearLeftPoint, $ClientDrawer::BoundingBoxWidth[%currentObj]);
                }
                return;
            }

            %lostAttachment = false;

            %editor.consoleFrameColor = %this.lineColor;
            %editor.consoleFillColor = %this.fillColor;

            // Each drawer would have at least this attachment if attached
            if (%this.attachment $= "raycast")
            {
                %controlled = ServerConnection.getControlObject();

                %start = %controlled.getMuzzlePoint($WeaponSlot);
                %dir = %controlled.getMuzzleVector($WeaponSlot);

                %dist = 200;
                %result = containerRayCast(%start, vectorAdd(%start, vectorScale(%dir, %dist)), $TypeMasks::TerrainObjectType, -1);
                %resultPos = getWords(%result, 1, 3);
                %result = getWord(%result, 0);

                // Just throw the marker into hell on frames where nothing
                // was struck.
                if (!isObject(%result))
                    %this.setTransform("0 0 0");
                else
                {
                    %this.setTransform(%resultPos);

                    // Render a line while we're at it
                    %editor.renderLine(%start, %resultPos, 10);
                }
            }
            else if (isObject(%this.attachment))
                %this.setTransform(%this.attachment.getWorldBoxCenter());
            else if (%this.attachment !$= "")
                %lostAttachment = true;

            switch(%this.clientDrawerType)
            {
                case $ClientDrawer::Type::Sphere:
                    %editor.renderSphere(%this.getWorldBoxCenter(), %this.radius, %this.tessellation);
                case $ClientDrawer::Type::Line:
                    %editor.renderLine(%this.getWorldBoxCenter(), %this.end, %this.width);
                case $ClientDrawer::Type::Circle:
                    // Crash fix: Shouldn't be < 0, but it doesn't really work if less than 6 either
                    if (%this.segments < 6)
                        %this.segments = 6;

                    %editor.renderCircle(%this.getWorldBoxCenter(), %this.normal, %this.radius, %this.segments);
                case $ClientDrawer::Type::PointTriangle:
                    %editor.renderTriangle(%this.getWorldBoxCenter(), %this.pointTwo, %this.pointThree);
            }

            if (%lostAttachment)
            {
                error("!!! ClientDraw: Client drawer of type " @ %this.clientDrawerType @ " lost its attachment. Assuming deletion.");

                MissionGroup.remove(%this);
                %this.delete();
            }
        }
    }
};

//--------------------------------------------------------------------------------------------------
//  This is an auxilliary hook for the client to hook into more common systems in the game.
//==================================================================================================
package ClientHooks
{
    //--------------------------------------------------------------------------------------------------
    //  This function is hooked so that we can reset the clientside MissionGroup used to trick the
    //  engine into calling the ::onEditorRender function for each audio emitter we create on the client
    //  end.
    //==================================================================================================
    function clientCmdMissionStartPhase1(%seq, %missionName, %musicTrack)
    {
        parent::clientCmdMissionStartPhase1(%seq, %missionName, %musicTrack);

        // Used for a listen server to recognize the correct control object
        memPatch("42E938", "9090");

        if (ServerConnection.getAddress() !$= "local")
        {
            memPatch("58B26D", "cc9c");
            memPatch("4376B7", "9090");
            memPatch("43745D", "9090");
            memPatch("42E938", "9090");
            memPatch("5E2EEC", "9090");
            memPatch("602AF3", "9090");
            memPatch("58C24C", "9090");
            memPatch("6B40D0", "9090");
            memPatch("5E34B6", "9090909090909090909090909090909090909090909090909090");
        }
    }

    //--------------------------------------------------------------------------------------------------
    // We hook the EditorGUI functions to properly hide the custom drawers from the editor preview.
    //==================================================================================================
    function EditorGui::onWake(%this)
    {
        parent::onWake(%this);

        if (isActivePackage(DrawSubsystemHooks))
            deactivatePackage(DrawSubsystemHooks);
    }

    //--------------------------------------------------------------------------------------------------
    //  We hook the EditorGUI functions to properly hide the custom drawers from the editor preview.
    //==================================================================================================
    function EditorGui::onSleep(%this)
    {
        parent::onSleep(%this);

        if (!isActivePackage(DrawSubsystemHooks))
            activatePackage(DrawSubsystemHooks);
    }

    //--------------------------------------------------------------------------------------------------
    //  We hook the PlayGUI::onWake function to explicitly take care of a sizing bug on the geometry
    //  overlay.
    //==================================================================================================
    function PlayGUI::onWake(%this)
    {
        parent::onWake(%this);

        // Small hack to make sure the PlayGUIEditor is sized correctly...
        // Apparently the PlayGUI starts with a size of 640, 480 and only changes its extents *shortly* after this
        // ::onWake function is called, so we just force-size the hack GUI to our current
        // game resolution.
        PlayGUIEditor.setExtent(getWord($pref::Video::resolution, 0), getWord($pref::Video::resolution, 1));

        PlayGUI.bringToFront(PlayGUIEditor);
    }

    //--------------------------------------------------------------------------------------------------
    //  We hook the disconnect function so the client can use the regular PlayGUI by default. We also
    //  use it to restore the default T2 code so that a client can leave a server using the custom
    //  drawers and run a listen server immediately afterwards.
    //==================================================================================================
    function disconnect()
    {
        parent::disconnect();

        if (isObject(MissionGroup))
            MissionGroup.DrawerCount = 0;

        clientCmdUseRegularPlayGUI();

        // We didn't patch it to begin with.
        if (ServerConnection.getAddress() !$= "local")
        {
            memPatch("58B26D", "3c8a");
            memPatch("4376B7", "7501");
            memPatch("43745D", "7501");
            memPatch("42E938", "7501");
            memPatch("5E2EEC", "751c");
            memPatch("602AF3", "7407");
            memPatch("58C24C", "7414");
            memPatch("6B40D0", "741f");
            memPatch("5E34B6", "8b414083e00274126888dc7900e8282be4ff31c05989ec5dc390");
        }
    }

    //--------------------------------------------------------------------------------------------------
    //  We hook the tiggleZoom function because the geometry overlay can't zoom. So, if TSExtension
    //  is available, we use a hack to force zooming. If it isn't, we just hide the hack overlay for
    //  the duration of the zoom.
    //==================================================================================================
    function toggleZoom(%pressed)
    {
        parent::toggleZoom(%pressed);

        if (PlayGUI.getUsingClientDrawGUI() && !$TSExtension::isActive)
            PlayGUIEditor.setVisible(!%pressed);
    }

    //--------------------------------------------------------------------------------------------------
    //  We hook the setFOV function so we can properly set the FOV when using the hack overlay.
    //==================================================================================================
    function setFov(%fov)
    {
        if (PlayGUI.getUsingClientDrawGUI() && $TSExtension::isActive)
            setEditFovHex(floatToHex(mDegToRad(%fov)));

        Parent::setFov(%fov);
    }
};

//--------------------------------------------------------------------------------------------------
//
//==================================================================================================
function PlayGUI::getUsingClientDrawGUI(%this)
{
    return PlayGUIEditor.isVisible();
}

// Helper function proided by Bahke.
function fovByte(%hexstr, %byte)
{
    return getSubStr(%hexstr, %byte * 2, 2);
}

// Helper function proided by Bahke.
function setEditFovHex(%hexfovfloat)
{
    %hexstr = %hexfovfloat;
    memPatch("467E98", fovByte(%hexstr, 3) @ fovByte(%hexstr, 2) @ fovByte(%hexstr, 1) @ fovByte(%hexstr, 0));
}
