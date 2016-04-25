using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Downplay.Orchard.LayoutSelector.Models;
using Orchard.ContentManagement.Drivers;
using Orchard;
using Downplay.Orchard.LayoutSelector.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;

namespace Downplay.Orchard.LayoutSelector.Drivers
{
    public class LayoutSelectorPartDriver : ContentPartDriver<LayoutSelectorPart> {
        private readonly ILayoutSelectorService _layoutService;
        private readonly IWorkContextAccessor _workContextAccessor;

        public LayoutSelectorPartDriver(IWorkContextAccessor workContextAccessor, ILayoutSelectorService layoutService)
        {
  
            _workContextAccessor = workContextAccessor;
            _layoutService = layoutService;
        }

        //DISPLAY
        protected override DriverResult Display(LayoutSelectorPart part, string displayType, dynamic shapeHelper)
        {
            if (!String.IsNullOrWhiteSpace(part.LayoutName) && displayType == "Detail")
            {
                // Get WorkContext
                var wc = _workContextAccessor.GetContext();
                // Add the alternate name to Layout's Metadata. Double underscore '__' translate to a hyphen '-' in the file name.
                wc.Layout.Metadata.Alternates.Add("Layout__"+part.LayoutName);
            }

            // This part doesn't usually display anything
            return null;
        }

        //GET
        protected override DriverResult Editor(LayoutSelectorPart part, dynamic shapeHelper)
        {
            // Make layout names available to drop-down list
            part.AvailableLayouts = _layoutService.GetLayouts();
            return ContentShape("Parts_LayoutSelector_Edit",
                     () => shapeHelper.EditorTemplate(
                         TemplateName: "Parts/LayoutSelector",
                         Model: part,
                //         AvailableLayouts: _layoutService.GetLayouts(),
                         Prefix: Prefix));
        }

        //POST
        protected override DriverResult Editor(LayoutSelectorPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            // This line updates your part from the POST fields.
            updater.TryUpdateModel(part, Prefix, null, null);
            
            // If you need to do any custom processing, you can do so here.

            // Now just display the same editor as before
            return Editor(part, shapeHelper);
        }
        
        protected override void Importing(LayoutSelectorPart part, ImportContentContext context) {
            // Don't do anything if the tag is not specified.
            if (context.Data.Element(part.PartDefinition.Name) == null) {
                return;
            }

            context.ImportAttribute(part.PartDefinition.Name, "LayoutName", layoutName =>
                part.LayoutName = layoutName
            );
        }

        protected override void Exporting(LayoutSelectorPart part, ExportContentContext context) {
            if (part.LayoutName != null) {
                context.Element(part.PartDefinition.Name).SetAttributeValue("LayoutName", part.LayoutName);
            }
        }

    }
}
/*
            if (displayType == "SummaryAdmin")
            {
                // Hide when in Summary Admin (for now)
                return null;
            }
            // Experimental;
            //            var wc = _workContextAccessor.GetContext;
            //          wc.Layout.Metadata.Alternates.Add("Layout_Foo");

*/