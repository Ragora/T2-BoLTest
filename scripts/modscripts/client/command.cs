package BOLBinds
{
    function OptionsDlg::onWake(%this)
    {
        if (!$BOL::BoundKeys)
        {
            $RemapName[$RemapCount] = "Ray Action";
            $RemapCmd[$RemapCount] = "RayAction";
            $RemapCount++;
        }

        $BOL::BoundKeys = true;
        parent::onWake(%this);
    }
};

function RayAction(%val)
{
    if (%val)
        commandToServer('RayAction');
}

if (!isActivePackage(BOLBinds))
    activatePackage(BOLBinds);
