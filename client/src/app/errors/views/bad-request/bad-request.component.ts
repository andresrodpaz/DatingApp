import { Component, OnInit } from '@angular/core';
import { trigger, transition, animate, style } from '@angular/animations';
@Component({
  selector: 'app-bad-request',
  templateUrl: './bad-request.component.html',
  styleUrls: ['./bad-request.component.css'],
  animations: [
    trigger('glitchError', [
      transition('* => *', [
        style({ transform: 'none', opacity: 1 }),
        animate('2.5s', style({ transform: 'skew(-0.5deg, -0.9deg)', opacity: 0.75 })),
        animate('2.5s', style({ transform: 'none', opacity: 1 })),
        animate('2.5s', style({ transform: 'none', opacity: 1 })),
        animate('2.5s', style({ transform: 'skew(0.8deg, -0.1deg)', opacity: 0.75 })),
        animate('2.5s', style({ transform: 'none', opacity: 1 })),
        animate('2.5s', style({ transform: 'none', opacity: 1 })),
        animate('2.5s', style({ transform: 'skew(-1deg, 0.2deg)', opacity: 0.75 })),
        animate('2.5s', style({ transform: 'none', opacity: 1 })),
        animate('2.5s', style({ transform: 'none', opacity: 1 })),
        animate('2.5s', style({ transform: 'skew(0.4deg, 1deg)', opacity: 0.75 })),
        animate('2.5s', style({ transform: 'none', opacity: 1 }))
      ])
    ])
  ]
})
export class BadRequestComponent  implements OnInit {

  ngOnInit(): void {
    const stackContainer = document.querySelector('.stack-container');
    const cardNodes = document.querySelectorAll('.card-container');
    const consoleNodes = document.querySelectorAll('.writing');
    const perspecNodes = document.querySelectorAll('.perspec');
    const perspec = document.querySelector('.perspec');
    const card = document.querySelector('.card');

    let counter = (stackContainer as HTMLElement).children.length;

    //function to generate random number
    function randomIntFromInterval(min: number, max: number): number {
      return Math.floor(Math.random() * (max - min + 1) + min);
    }

    if (card instanceof HTMLElement) {
      //after tilt animation, fire the explode animation
      card.addEventListener('animationend', () => {
        for (let i = 0; i < perspecNodes.length; i++) {
          const elem = perspecNodes[i];
          if (elem instanceof HTMLElement) {
            elem.classList.add('explode');
          }
        }
      });
    }

    if (perspec instanceof HTMLElement) {
      //after explode animation do a bunch of stuff
      perspec.addEventListener('animationend', (e: Event) => {
        if ((e as AnimationEvent).animationName === 'explode') {
          for (let i = 0; i < cardNodes.length; i++) {
            const elem = cardNodes[i];
            if (elem instanceof HTMLElement) {
              //add hover animation class
              elem.classList.add('pokeup');

              //add event listener to throw card on click
              elem.addEventListener('click', () => {
                let updown = [800, -800];
                let randomY = updown[Math.floor(Math.random() * updown.length)];
                let randomX = Math.floor(Math.random() * 1000) - 1000;
                elem.style.transform = `translate(${randomX}px, ${randomY}px) rotate(-540deg)`;
                elem.style.transition = "transform 1s ease, opacity 2s";
                elem.style.opacity = "0";
                counter--;
                if (counter === 0) {
                  (stackContainer as HTMLElement).style.width = "0";
                  (stackContainer as HTMLElement).style.height = "0";
                }
              });

              //generate random number of lines of code between 4 and 10 and add to each card
              let numLines = randomIntFromInterval(5, 10);

              //loop through the lines and add them to the DOM
              for (let index = 0; index < numLines; index++) {
                let lineLength = randomIntFromInterval(25, 97);
                var node = document.createElement("li");
                node.classList.add('node-' + index);

                elem.querySelector('.code ul')?.appendChild(node).setAttribute('style', '--linelength: ' + lineLength + '%;');

                //draw lines of code 1 by 1
                if (index == 0) {
                  elem.querySelector('.code ul .node-' + index)?.classList.add('writeLine');
                } else {
                  elem.querySelector('.code ul .node-' + (index - 1))?.addEventListener('animationend', (e: Event) => {
                    if ((e as AnimationEvent).animationName === 'writeLine') {
                      if (index == numLines - 1) {
                        node.classList.add('errorLine');
                        elem.querySelector('.code ul .node-' + index)?.classList.add('writeLine');
                        setTimeout(() => {
                          for (let j = 0; j < consoleNodes.length; j++) {
                            const consoleElem = consoleNodes[j];
                            if (consoleElem instanceof HTMLElement) {
                              consoleElem.classList.add('writing-error');
                            }
                          }
                        }, 2000);
                      } else {
                        elem.querySelector('.code ul .node-' + index)?.classList.add('writeLine');
                      }
                    }
                  });
                }
              }
            }
          }
        }
      });
    }
  }
}
