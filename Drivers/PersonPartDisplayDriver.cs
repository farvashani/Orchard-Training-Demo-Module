using System.Threading.Tasks;
using Lombiq.TrainingDemo.Models;
using Lombiq.TrainingDemo.ViewModels;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;

namespace Lombiq.TrainingDemo.Drivers
{
    // Drivers inherited from ContentPartDisplayDrivers have a functionality similar to the one described in
    // BookDisplayDriver but these are for ContentParts. Don't forget to register this class with the service provider
    // (see: Startup.cs).
    public class PersonPartDisplayDriver : ContentPartDisplayDriver<PersonPart>
    {
        // A Display method that we already know. This time it's much simpler because we don't want to create multiple
        // shapes for the PersonPart - however we could.
        public override IDisplayResult Display(PersonPart part) =>
            View(nameof(PersonPart), part).Location("Content: 1");

        // This is something that wasn't implemented in the BookDisplayDriver (but could've been). It will generate the
        // editor shape for the PersonPart.
        public override IDisplayResult Edit(PersonPart personPart)
        {
            // Something similar to the Display method happens: you have a shape helper with a shape name possibly and
            // a factory. For editing using Initialize is the best idea. It will instantiate a view model from a type
            // given as a generic parameter. In the factory you will map the content part properties to the view model.
            return Initialize<PersonPartViewModel>("PersonPart_Edit", model =>
            {
                model.PersonPart = personPart;
                
                model.BirthDateUtc = personPart.BirthDateUtc;
                model.Name = personPart.Name;
                model.Handedness = personPart.Handedness;
            }).Location("Content:1");
        }

        // NEXT STATION: Startup.cs and find the static constructor.

        // So we had an Edit (or EditAsync) that generates the editor shape now it's time to do the content
        // part-specific model binding and validation.
        public override async Task<IDisplayResult> UpdateAsync(PersonPart model, IUpdateModel updater)
        {
            var viewModel = new PersonPartViewModel();

            // Now it's where the IUpdateModel interface is really used (remember we first used it in
            // DisplayManagementController?). With this you will be able to use the Controller's model binding helpers
            // here in the driver. The prefix property will be used to distinguish between similarly named input fields
            // when building the editor form (so e.g. two content parts composing a content item can have an input
            // field called "Name"). By default Orchard Core will use the content part name but if you have multiple
            // drivers with editors for a content part you need to override it in the driver.
            await updater.TryUpdateModelAsync(viewModel, Prefix);

            // Now you can do some validation if needed. One way to do it you can simply write your own validation here
            // or you can do it in the view model class.

            // Go and check the ViewModels/PersonPartViewModel to see how to do it and then come back here.

            // Finally map the view model to the content part. By default these changes won't be persisted if there was
            // a validation error. Otherwise these will be automatically stored in the database.
            model.BirthDateUtc = viewModel.BirthDateUtc;
            model.Name = viewModel.Name;
            model.Handedness = viewModel.Handedness;
            
            return Edit(model);
        }
    }
}

// NEXT STATION: Controllers/PersonListController and go back to the OlderThan30 method where we left.