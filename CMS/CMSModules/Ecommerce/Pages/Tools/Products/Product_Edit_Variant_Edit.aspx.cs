using System;
using System.Collections.Generic;
using System.Linq;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.UIControls;

[EditedObject(SKUInfo.OBJECT_TYPE_VARIANT, "variantId")]
[ParentObject(SKUInfo.OBJECT_TYPE_SKU, "productId")]
[Title("com.products.variantproperties")]
[Breadcrumb(0, "com.products.variants", "Product_Edit_Variants.aspx?productId={%EditedObjectParent.ID%}&dialog={?dialog?}", null)]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
[UIElement(ModuleName.ECOMMERCE, "Products.Variants")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Variant_Edit : CMSProductsPage
{
    #region "Variables"

    private readonly Dictionary<string, SKUInfo> options = new Dictionary<string, SKUInfo>();

    #endregion


    #region "Properties"

    private SKUInfo Variant
    {
        get
        {
            return EditedObject as SKUInfo;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        editForm.OnBeforeDataLoad += editForm_OnBeforeDataLoad;
        editForm.OnCheckPermissions += editForm_OnCheckPermissions;
    }


    /// <summary>
    /// Checks if current user is authorized to modify product variant.
    /// </summary>
    protected void editForm_OnCheckPermissions(object sender, EventArgs e)
    {
        if ((Variant != null) && (Variant.Parent != null))
        {
            // Check parent product permissions
            CheckProductModifyAndRedirect((SKUInfo)Variant.Parent);
        }
    }


    protected void editForm_OnBeforeDataLoad(object sender, EventArgs e)
    {
        if ((Variant == null) || !Variant.IsProductVariant)
        {
            EditedObject = null;
            return;
        }

        CheckEditedObjectSiteID(Variant.SKUSiteID);

        var optionsDs = SKUInfoProvider.GetSKUs()
              .WhereIn("SKUID",
                  VariantOptionInfoProvider.GetVariantOptions()
                    .Column("OptionSKUID")
                    .WhereEquals("VariantSKUID", Variant.SKUID)
               );

        SetVariantAttributes(optionsDs);
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Get label for info text field
        LocalizedLabel infoTextLabel = editForm.FieldLabels["InfoText"];

        // Set label to render without colon. It is used for info message through whole form width.
        if (infoTextLabel != null)
        {
            infoTextLabel.DisplayColon = false;
        }

        foreach (KeyValuePair<string, SKUInfo> item in options)
        {
            // Fill attributes labels
            if (editForm.FieldControls[item.Key] != null)
            {
                editForm.FieldControls[item.Key].Value = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(item.Value.SKUName));
            }
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Creates category with variant attributes.
    /// </summary>
    /// <param name="optionsDs">Product options</param>
    private void SetVariantAttributes(IEnumerable<SKUInfo> optionsDs)
    {
        // Get attributes category index - just before representing category
        var attrPos = editForm.FormInformation.ItemsList.FindIndex(f =>
            (f is FormCategoryInfo) && ((FormCategoryInfo)f).CategoryName.EqualsCSafe("com.sku.representingcategory"));


        // Create attributes category
        var attCategory = new FormCategoryInfo()
        {
            CategoryName = "Attributes",
            IsDummy = true,
        };

        attCategory.SetPropertyValue(FormCategoryPropertyEnum.Caption, HTMLHelper.HTMLEncode(GetString("com.variant.attributes")));
        editForm.FormInformation.AddFormItem(attCategory, attrPos++);

        foreach (var option in optionsDs)
        {
            if (option.Parent != null)
            {
                string categoryCodeName = option.Parent.Generalized.ObjectCodeName;
                options.Add(categoryCodeName, option);

                FormFieldInfo ffOption = new FormFieldInfo
                {
                    Name = categoryCodeName,
                    AllowEmpty = true,
                    Size = 400,
                    FieldType = FormFieldControlTypeEnum.LabelControl,
                    DataType = FieldDataType.Text,
                    IsDummyField = true,
                };

                OptionCategoryInfo parentOptionCategory = (OptionCategoryInfo)option.Parent;

                ffOption.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, HTMLHelper.HTMLEncode(ResHelper.LocalizeString(option.SKUName)));

                // Show category live site display name instead of category display name in case it is available
                ffOption.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, HTMLHelper.HTMLEncode(ResHelper.LocalizeString(parentOptionCategory.CategoryTitle)));

                //Insert field to the form on specified position
                editForm.FormInformation.AddFormItem(ffOption, attrPos++);
            }
        }
    }

    #endregion
}