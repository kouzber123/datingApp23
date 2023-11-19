import { CommonModule } from '@angular/common';
import { Component, OnInit, Input, NgModule, ViewChild, ChangeDetectionStrategy } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
import { MessageService } from 'src/app/_services/message.service';

@Component({

  selector: 'app-member-messages',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  standalone: true,
  imports: [CommonModule, TimeagoModule, FormsModule],
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm?: NgForm;
  @Input() username?: string;
  messageContent = '';
  //we get our messages straight from the service signalR
  constructor(public messageService: MessageService) {}
  ngOnInit(): void {}

  sendMessage() {
    if (!this.username) return;

    this.messageService
      .sendMessage(this.username, this.messageContent)
      .then(() => {
        this.messageForm?.reset();
      });
  }
}
