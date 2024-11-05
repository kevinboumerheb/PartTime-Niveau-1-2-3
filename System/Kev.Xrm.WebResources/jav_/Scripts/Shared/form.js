var Javista = Javista || {};
Javista.Utils = Javista.Utils || {};
Javista.Utils.Form = function () {
    function filterOptionSet(formContext, attribute, allValues, values) {
        ///<summary>
        /// Filtre un contr�le de type OptionSet en ne pr�sentant que les valeurs dans "values"
        ///</summary>
        ///<param name="formContext" type="object">
        /// Contexte du formulaire.
        ///</param>
        ///<param name="attribute" type="String">
        /// Nom de l'attribut pour le contr�le OptionSet � filtrer.
        ///</param>
        ///<param name="allValues" type="Array">
        /// Liste de toutes les options du contr�le OptionSet.
        ///</param>
        ///<param name="values" type="Array">
        /// Liste des options � pr�senter dnas le contr�le OptionSet.
        ///</param>
        var ctrl = formContext.getControl(attribute);
        var attr = formContext.getAttribute(attribute);

        if (!attr) return;

        var currentOption = attr.getValue();
        ctrl.clearOptions();

        for (let i = 0; i < allValues.length; i++) {
            if (values.includes(allValues[i].value)) {
                ctrl.addOption(allValues[i]);
            }
        }

        if (values.includes(currentOption)) {
            attr.setValue(currentOption);
        }
    }

    function setLookupValue(formContext, attribute, id, type, name) {
        ///<summary>
        /// D�finit une valeur d'un champ Lookup
        ///</summary>
        ///<param name="formContext" type="object">
        /// Contexte du formulaire.
        ///</param>
        ///<param name="attribute" type="String">
        /// Nom de l'attribut Lookup � d�finir.
        ///</param>
        ///<param name="id" type="String">
        /// Identifiant de l'enregistrement � lier.
        ///</param>
        ///<param name="type" type="String">
        /// Type de l'enregistrement � lier.
        ///</param>
        ///<param name="name" type="String">
        /// Nom d'affichage de l'enregistrement � lier.
        ///</param>
        var item = {
            id: id,
            entityType: type,
            name: name
        };
        var array = new Array();
        array.push(item);

        var attr = formContext.getAttribute(attribute);
        if (attr) {
            attr.setValue(array);
        }
    }

    function setSectionVisibility(formContext, tabName, sectionName, isVisible) {
        ///<summary>
        /// D�finit la visibilit� d'une section
        ///</summary>
        ///<param name="formContext" type="object">
        /// Contexte du formulaire.
        ///</param>
        ///<param name="tabName" type="String">
        /// Nom de l'onglet contenant la section.
        ///</param>
        ///<param name="sectionName" type="String">
        /// Nom de la section.
        ///</param>
        ///<param name="isVisible" type="Boolean">
        /// Indicateur de visibilit�.
        ///</param>
        var tab = formContext.ui.tabs.get(tabName);
        if (!tab) return;

        var section = tab.sections.get(sectionName);
        if (!section) return;

        section.setVisible(isVisible);
    }

    function setTabVisibility(formContext, tabName, isVisible) {
        ///<summary>
        /// D�finit la visibilit� d'une section
        ///</summary>
        ///<param name="formContext" type="object">
        /// Contexte du formulaire.
        ///</param>
        ///<param name="tabName" type="String">
        /// Nom de l'onglet.
        ///</param>
        ///<param name="isVisible" type="Boolean">
        /// Indicateur de visibilit�.
        ///</param>
        var tab = formContext.ui.tabs.get(tabName);
        if (!tab) return;

        tab.setVisible(isVisible);
    }

    function setControlsVisibility(formContext, isVisible, arrayOfControls) {
        ///<summary>
        /// D�finit la visibilit� d'une liste de contr�les
        ///</summary>
        ///<param name="formContext" type="object">
        /// Contexte du formulaire.
        ///</param>
        ///<param name="isVisible" type="Boolean">
        /// Indicateur de visibilit�.
        ///</param>
        ///<param name="arrayOfControls" type="String[]">
        /// Noms des contr�les.
        ///</param>
        for (let i = 0; i < arrayOfControls.length; i++) {
            var ctrl = formContext.getControl(arrayOfControls[i]);
            if (ctrl) ctrl.setVisible(isVisible);
        }
    }

    function setAttributesRequiredLevel(formContext, level, arrayOfAttributes) {
        ///<summary>
        /// D�finit le niveau requis d'une liste d'attributs
        ///</summary>
        ///<param name="formContext" type="object">
        /// Contexte du formulaire.
        ///</param>
        ///<param name="level" type="String">
        /// Niveau requis ("none"/"recommended"/"required").
        ///</param>
        ///<param name="arrayOfControls" type="String[]">
        /// Noms des contr�les.
        ///</param>
        for (let i = 0; i < arrayOfAttributes.length; i++) {
            var attr = formContext.getAttribute(arrayOfAttributes[i]);
            if (attr) attr.setRequiredLevel(level);
        }
    }

    return {
        FilterOptionSet: filterOptionSet,
        SetAttributesRequiredLevel: setAttributesRequiredLevel,
        SetLookupValue: setLookupValue,
        SetControlsVisibility: setControlsVisibility,
        SetSectionVisibility: setSectionVisibility,
        SetTabVisibility: setTabVisibility
    };
}();