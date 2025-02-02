using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CarouselSelector : BaseField<string>
{
    public new class UxmlFactory : UxmlFactory<CarouselSelector, UxmlTraits> { }

    public new class UxmlTraits : BaseFieldTraits<string, UxmlStringAttributeDescription>
    {
        private UxmlIntAttributeDescription index = new UxmlIntAttributeDescription
        {
            name = "index"
        };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            CarouselSelector carouselSelector = (CarouselSelector)ve;
            //carouselSelector.choices = BaseField<string>.UxmlTraits.ParseChoiceList
            carouselSelector.index = index.GetValueFromBag(bag, cc);
        }
    }

    private List<string> m_choices;
    private int m_index = -1;

    public List<string> choices
    {
        get => m_choices;
        set => m_choices = value;
    }

    public int index
    {
        get => m_index;
        set
        {
            m_index = value;
            bool invalidIndex = m_choices == null || value >= m_choices.Count || value < 0;
            this.value = invalidIndex ? null : m_choices[m_index];
        }
    }

    public override string value
    {
        get => base.value;
        set
        {
            m_index = m_choices?.IndexOf(value) ?? -1;
            base.value = value;
            if(currentChoiceLabel!=null)
                currentChoiceLabel.text = base.value;
        }
    }

    public static readonly new string ussClassName = "carousel-selector";
    public static readonly new string inputUssClassName = ussClassName + "__selector";
    public static readonly new string labelUssClassName = ussClassName + "__label";

    public static readonly string inputSelect = ussClassName + "__cycle-button";
    public static readonly string currentValueClassName = ussClassName + "__current-value";

    Label descriptionLabel;

    Button leftButton;
    Label currentChoiceLabel;
    Button rightButton;

    VisualElement selectorContainer;

    public CarouselSelector() : this(null) {}

    public CarouselSelector(string label) : base(label, null)
    {
        // style entire control
        AddToClassList(ussClassName);

        selectorContainer = this.Q(className: BaseField<string>.inputUssClassName);
        selectorContainer.AddToClassList(inputUssClassName);
        Add(selectorContainer);

        base.labelElement.AddToClassList(labelUssClassName);

        leftButton = new Button();
        leftButton.AddToClassList(inputSelect);
        leftButton.text = "<";
        leftButton.clicked += () => CycleCarousel(-1);
        Add(leftButton);

        currentChoiceLabel = new Label();
        currentChoiceLabel.AddToClassList(currentValueClassName);
        Add(currentChoiceLabel);

        rightButton = new Button();
        rightButton.AddToClassList(inputSelect);
        rightButton.text = ">";
        rightButton.clicked += () => CycleCarousel(1);
        Add(rightButton);

        choices = new();
    }

    void CycleCarousel(int dir) => index = (index + dir).Mod(choices.Count);
}
