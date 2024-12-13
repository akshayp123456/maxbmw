<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UC_Parts_Head.ascx.cs" Inherits="UC_Parts_Head" %>

<script src="js/jquery-contained-sticky-scroll.js" type="text/javascript"></script>

<link href="css/ComboBox.css" rel="stylesheet" type="text/css" />
<link href="css/ListProducts.css" rel="stylesheet" type="text/css" />

<script type="text/javascript">
function SetVehicleIcon(text1, text2, icon) {
    document.getElementById('VEHICLETEXT1').innerText = text1;
    document.getElementById('VEHICLETEXT2').innerText = 'Production: ' + text2;
    document.getElementById('VEHICLEICON').src = "VehiclesIcons/" + icon + ".jpg?v=<%=Utilities.VERSION %>";
}

function getUrlVars(url) {
    var vars = [], hash;
    var hashes = url.slice(url.indexOf('?') + 1).split('&');
    for(var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0].toLowerCase());
        vars[hash[0].toLowerCase()] = hash[1];
    }
    return vars;
}

function ShowPartsDetails(Diagram, PartNumber) {
    $('#Loading').show();

    var currentParams = getUrlVars(window.location.href);
        
    var vid = currentParams['vid'];
    if (vid==null) vid = '';
            
    var category = currentParams['category'];
    if (category==null) category = '';
            
    var brand = currentParams['brand'];
    if (brand == null) brand = '';

    var viewmode = currentParams['viewmode'];
    if (viewmode == null) viewmode = '';
                        
    // build the url with the parameters
    var params = '';
    params += 'source=<%=this.Page.ToString().ToLower().Replace("asp.parts","").Replace("_aspx","") %>'; // source always has to be present     
    params += (vid != '' ? '&vid=' + vid : '');
    params += (category != '' ? '&category=' + escape(unescape(category)) : '');
    params += (brand != '' ? '&brand=' + escape(unescape(brand)) : '');
            
    params += (Diagram != '' ? '&diagram=' + escape(unescape(Diagram)) : '');
    params += (PartNumber != '' ? '&partnumber=' + escape(unescape(PartNumber)) : '');

    params += (viewmode != '' ? '&viewmode=' + viewmode : '');           
    
    window.location = 'PartsDetails.aspx' + (params != '' ? '?' + params : '');
}

function UpdateResults() {
    $('#Loading').show();

    var vid = $('#comboBoxModels').val();
    var category = $('#comboBoxCategories').val();
    var brand = $('#comboBoxBrands').val();

    // blank them in case of "...: All ..."
    if (vid.indexOf(':') >= 0) vid = '';
    if (brand.indexOf(':') >= 0) brand = '';
    if (category.indexOf(':') >= 0) category = '';
    if (category.indexOf('Front &amp; Rear') >= 0) category = '';

    // build the url with the parameters
    var params = '';
    params += (vid != '' ? (params != "" ? "&" : "") + 'vid=' + escape(vid) : '');
    params += (category != '' ? (params != "" ? "&" : "") + 'category=' + escape(category) : '');
    params += (brand != '' ? (params != "" ? "&" : "") + 'brand=' + escape(brand) : '');

    if ($('#ViewModeList').css('border-left-color') == 'rgb(235, 63, 60)') // rgb(204, 51, 51) is the same as #cc3333  /   eb3f3c=235,63,60
        params += '&viewmode=list';

    window.location = '<%=this.Page.ToString().ToLower().Replace("asp.","").Replace("_",".") %>' + (params != '' ? '?' + params : '');            
}

function BikeModelClicked(model, vid) {
    // catalog or tires was selected (apparel has no models), so update the iframe only
    $('#comboBoxModels').parent().children('.ComboBoxText').text(model);
    $('#comboBoxModels').parent().children('.ComboBoxText').css('background-color', '#ccc');
    $('#comboBoxModels').parent().children('.ComboBoxX').show();
    $('#comboBoxModels').val(vid);
    UpdateResults();
}
</script>