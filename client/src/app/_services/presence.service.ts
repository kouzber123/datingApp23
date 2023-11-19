import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { BehaviorSubject, take } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable(); // we this we can subs
  private hubConnection?: HubConnection;


  constructor(private toastR: ToastrService, private router: Router) {}

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .build(); //build connection

    this.hubConnection.start().catch((err) => console.log(err));

    this.hubConnection.on('UserIsOnline', (username) => {
        this.onlineUsers$.pipe(take(1)).subscribe({
          next: usernames => this.onlineUsersSource.next([...usernames, username])
        })
    });

    this.hubConnection.on('UserIsOffline', (username) => {
      this.onlineUsers$.pipe(take(1)).subscribe({
        next: usernames => this.onlineUsersSource.next(usernames.filter(x => x !== username))
      })
    });

    this.hubConnection.on('GetOnlineUsers', (onlineUsers) => {
      this.onlineUsersSource.next(onlineUsers);
    });

    this.hubConnection.on('NewMessageReceived', (sender: User) => {
      this.toastR
        .info(
          'You received Message from ' +
            sender.userName +
            ' known as ' +
            sender.knownAs
        )
        .onTap.pipe(take(1))
        .subscribe({
          next: () => {
            this.router.navigateByUrl(
              '/members/' + sender.userName + '?tab=Messages'
            );
          },
        });
    });
  }

  stopHubConnection() {
    this.hubConnection?.stop().catch((error) => console.log(error));
  }
}
