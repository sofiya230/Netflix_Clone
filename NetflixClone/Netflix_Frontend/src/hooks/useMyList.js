import { useEffect, useState, useCallback } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { myListService } from '../services/api';

export default function useMyList(contentId) {
  const { currentProfile } = useAuth();
  const [inList, setInList] = useState(false);
  const [busy, setBusy] = useState(false);

  // check if this content is in the current profile's list
  useEffect(() => {
    let cancelled = false;

    const check = async () => {
      if (!currentProfile?.id || !contentId) return;
      try {
        const res = await myListService.isContentInMyList(currentProfile.id, contentId);
        // BE can return boolean or object; treat truthy as "in list"
        if (!cancelled) setInList(!!res || res?.isInList === true);
      } catch (e) {
        if (!cancelled) setInList(false);
      }
    };

    check();
    return () => { cancelled = true; };
  }, [currentProfile?.id, contentId]);

  const toggle = useCallback(async () => {
    if (!currentProfile?.id || !contentId || busy) return;
    setBusy(true);
    const profileId = currentProfile.id;

    try {
      if (inList) {
        // remove
        await myListService.removeFromMyList(profileId, contentId);
        setInList(false);
      } else {
        // add
        await myListService.addToMyList(profileId, contentId);
        setInList(true);
      }
            } catch (e) {
        } finally {
      setBusy(false);
    }
  }, [currentProfile?.id, contentId, inList, busy]);

  return { inList, toggle, busy, profileId: currentProfile?.id };
}
